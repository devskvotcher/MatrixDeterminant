using System;
using System.Data.SQLite;
using System.Text;
using System.Windows.Forms;

public class DBForm : Form
{
    private Button btnSaveToDB, btnLoadFromDB, deleteButton;
    private ListBox listBox;
    private SQLiteConnection connection;
    private DataGridView dataGridView;
    public DBForm(DataGridView dataGridView1)
    {
        this.dataGridView = dataGridView1;
        InitializeComponent();
        CreateDatabase();
        LoadMatrixList();
    }

    private void InitializeComponent()
    {
            this.btnSaveToDB = new System.Windows.Forms.Button();
            this.btnLoadFromDB = new System.Windows.Forms.Button();
            this.deleteButton = new System.Windows.Forms.Button();
            this.listBox = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // btnSaveToDB
            // 
            this.btnSaveToDB.Location = new System.Drawing.Point(10, 10);
            this.btnSaveToDB.Name = "btnSaveToDB";
            this.btnSaveToDB.Size = new System.Drawing.Size(100, 50);
            this.btnSaveToDB.TabIndex = 0;
            this.btnSaveToDB.Text = "Save to DB";
            this.btnSaveToDB.Click += new System.EventHandler(this.btnSaveToDB_Click);
            // 
            // btnLoadFromDB
            // 
            this.btnLoadFromDB.Location = new System.Drawing.Point(120, 10);
            this.btnLoadFromDB.Name = "btnLoadFromDB";
            this.btnLoadFromDB.Size = new System.Drawing.Size(100, 50);
            this.btnLoadFromDB.TabIndex = 1;
            this.btnLoadFromDB.Text = "Load from DB";
            this.btnLoadFromDB.Click += new System.EventHandler(this.btnLoadFromDB_Click);
        // 
        // deleteButton
        // 
        this.deleteButton.Location = new System.Drawing.Point(220, 10);
        this.deleteButton.Name = "deleteButton";
        this.deleteButton.Size = new System.Drawing.Size(100, 50);
        this.deleteButton.TabIndex = 1;
        this.deleteButton.Text = "Delete from DB";
        this.deleteButton.Click += new System.EventHandler(this.deleteButton_Click);
        // 
        // listBox
        // 
        this.listBox.ItemHeight = 20;
            this.listBox.Location = new System.Drawing.Point(10, 70);
            this.listBox.Name = "listBox";
            this.listBox.Size = new System.Drawing.Size(210, 104);
            this.listBox.TabIndex = 2;
            // 
            // DBForm
            // 
            this.ClientSize = new System.Drawing.Size(392, 324);
            this.Controls.Add(this.btnSaveToDB);
            this.Controls.Add(this.btnLoadFromDB);
            this.Controls.Add(this.deleteButton);
            this.Controls.Add(this.listBox);
            this.Name = "DBForm";
            this.ResumeLayout(false);

    }

    private void btnSaveToDB_Click(object sender, EventArgs e)
    {
        // Проверка на то, что DataGridView не пуст
        if (dataGridView.RowCount > 0 && dataGridView.ColumnCount > 0)
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < dataGridView.RowCount; i++)
            {
                for (int j = 0; j < dataGridView.ColumnCount; j++)
                {
                    if (dataGridView.Rows[i].Cells[j].Value != null && dataGridView.Rows[i].Cells[j].Value.ToString() != string.Empty)
                    {
                        sb.Append(dataGridView.Rows[i].Cells[j].Value);
                        if (j < dataGridView.ColumnCount - 1)
                            sb.Append(";");
                    }
                }
                if (i < dataGridView.RowCount - 1)
                    sb.Append(" ");
            }

            string matrixAsString = sb.ToString();

            // Сохраняем строку в базе данных
            string sql = "INSERT INTO matrices (matrix) VALUES (@matrix)";
            SQLiteCommand command = new SQLiteCommand(sql, connection);
            command.Parameters.AddWithValue("@matrix", matrixAsString);

            int rowsAffected = command.ExecuteNonQuery();
            if (rowsAffected == 0)
            {
                MessageBox.Show("No rows were inserted into the database.");
                return;
            }

          //  command.ExecuteNonQuery();


            // Обновляем список матриц
            LoadMatrixList();
        }
        else
        {
            MessageBox.Show("Matrix is empty. Please input the matrix first.");
        }
    }
    private void btnLoadFromDB_Click(object sender, EventArgs e)
    {
        dataGridView.Columns.Clear();
        dataGridView.Rows.Clear();
        if (listBox.SelectedItem == null)
        {
            MessageBox.Show("Please select a matrix from the list.");
            return;
        }

        // Получаем выбранную матрицу из списка
        string selectedMatrixItem = listBox.SelectedItem.ToString();

        // Загружаем строку матрицы из базы данных
        string sql = "SELECT matrix FROM matrices WHERE id = @id";
        SQLiteCommand command = new SQLiteCommand(sql, connection);

        string[] parts = selectedMatrixItem.Split(':');
        if (parts.Length >= 2)
        {
            string selectedMatrixId = parts[0].Trim();
            int matrixId;
            if (int.TryParse(selectedMatrixId, out matrixId))
            {
               // MessageBox.Show($"Selected Matrix ID: {selectedMatrixId}");
                command.Parameters.AddWithValue("@id", matrixId);
            }
            else
            {
                MessageBox.Show("Invalid matrix ID.");
                return;
            }
        }
        else
        {
            MessageBox.Show("Invalid matrix item in the list.");
            return;
        }

        SQLiteDataReader reader = command.ExecuteReader();

        if (reader.Read())
        {
            //MessageBox.Show("Read exe");
            string matrixAsString = reader["matrix"].ToString();

            // Проверяем, что мы правильно считали данные
           // MessageBox.Show($"Read from DB: {matrixAsString}");

            // Преобразуем строку обратно в матрицу
            string[] rows = matrixAsString.Split(' ');

            // Получаем количество столбцов из первой строки
            string[] firstRowCells = rows[0].Split(';');

            // Установка размера dataGridView
            while (dataGridView.ColumnCount < firstRowCells.Length)
            {
                dataGridView.Columns.Add(new DataGridViewTextBoxColumn());
            }
            while (dataGridView.ColumnCount > firstRowCells.Length)
            {
                dataGridView.Columns.RemoveAt(dataGridView.ColumnCount - 1);
            }

            while (dataGridView.RowCount < rows.Length)
            {
                dataGridView.Rows.Add();
            }
            while (dataGridView.RowCount > rows.Length)
            {
                dataGridView.Rows.RemoveAt(dataGridView.RowCount - 1);
            }

            // Заполнение dataGridView
            for (int i = 0; i < rows.Length; i++)
            {
                string[] cells = rows[i].Split(';');
                for (int j = 0; j < cells.Length; j++)
                {
                    dataGridView.Rows[i].Cells[j].Value = cells[j];
                }
            }
        }

        reader.Close();
    }
    private void deleteButton_Click(object sender, EventArgs e)
    {
        // Проверяем, выбрана ли строка в listBox
        if (listBox.SelectedItem != null)
        {
            // Получаем id записи из выбранной строки (предполагается, что строки в listBox имеют формат "id: matrix")
            string selectedItem = listBox.SelectedItem.ToString();
            string selectedId = selectedItem.Split(':')[0].Trim();

            // Создаем команду удаления
            string sql = "DELETE FROM matrices WHERE id = @id";
            SQLiteCommand command = new SQLiteCommand(sql, connection);

            // Добавляем параметр id в команду
            command.Parameters.AddWithValue("@id", selectedId);

            // Выполняем команду
            int rowsAffected = command.ExecuteNonQuery();

            // Проверяем, была ли удалена какая-либо строка
            if (rowsAffected > 0)
            {
                MessageBox.Show("Запись успешно удалена!");
            }
            else
            {
                MessageBox.Show("Не удалось удалить запись. Возможно, она уже была удалена.");
            }
            //TODO Если нужно будет расширять таблицы удалить сслыку на этот метод
            RenumberIds();
            // Обновляем список
            LoadMatrixList();
        }
        else
        {
            MessageBox.Show("Пожалуйста, выберите запись для удаления.");
        }
    }
    private void LoadMatrixList()
    {
        // Загружаем список матриц из базы данных
        string sql = "SELECT * FROM matrices"; // Изменено на SELECT *, чтобы получить все столбцы
        SQLiteCommand command = new SQLiteCommand(sql, connection);
        SQLiteDataReader reader = command.ExecuteReader();

        listBox.Items.Clear();

        while (reader.Read())
        {
            listBox.Items.Add(reader["id"].ToString() + ": " + reader["matrix"].ToString()); // Отображаем и id, и matrix
        }
    }
    private void RenumberIds()
    {
        using (SQLiteTransaction transaction = connection.BeginTransaction())
        {
            try
            {
                // Создаем новую таблицу с такой же структурой, как и исходная
                string createTableSql = "CREATE TABLE matrices_new (id INTEGER PRIMARY KEY AUTOINCREMENT, matrix TEXT NOT NULL)";
                using (SQLiteCommand createTableCommand = new SQLiteCommand(createTableSql, connection))
                {
                    createTableCommand.ExecuteNonQuery();
                }

                // Копируем данные из исходной таблицы в новую, пропуская столбец id
                string copyDataSql = "INSERT INTO matrices_new (matrix) SELECT matrix FROM matrices";
                using (SQLiteCommand copyDataCommand = new SQLiteCommand(copyDataSql, connection))
                {
                    copyDataCommand.ExecuteNonQuery();
                }

                // Удаляем исходную таблицу
                string dropTableSql = "DROP TABLE matrices";
                using (SQLiteCommand dropTableCommand = new SQLiteCommand(dropTableSql, connection))
                {
                    dropTableCommand.ExecuteNonQuery();
                }

                // Переименовываем новую таблицу в исходную
                string renameTableSql = "ALTER TABLE matrices_new RENAME TO matrices";
                using (SQLiteCommand renameTableCommand = new SQLiteCommand(renameTableSql, connection))
                {
                    renameTableCommand.ExecuteNonQuery();
                }

                transaction.Commit();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error renumbering ids: {ex.Message}");
                transaction.Rollback();
            }
        }

        // Обновляем listBox
        LoadMatrixList();
    }

    private void CreateDatabase()
    {
        connection = new SQLiteConnection("Data Source=matrix.db; Version=3;");
        connection.Open();

        string sql = "CREATE TABLE IF NOT EXISTS matrices (id INTEGER PRIMARY KEY, matrix TEXT)";

        SQLiteCommand command = new SQLiteCommand(sql, connection);
        command.ExecuteNonQuery();
    }
}