using System;
using System.Data.SQLite;
using System.Windows.Forms;

public class DBForm : Form
{
    private Button btnSaveToDB, btnLoadFromDB;
    private ListBox listBox;
    private SQLiteConnection connection;
    public DBForm()
    {
        InitializeComponent();
        CreateDatabase();
        LoadMatrixList();
    }

    private void InitializeComponent()
    {
        this.btnSaveToDB = new Button();
        this.btnLoadFromDB = new Button();
        this.listBox = new ListBox();

        // 
        // btnSaveToDB
        // 
        this.btnSaveToDB.Location = new System.Drawing.Point(10, 10);
        this.btnSaveToDB.Size = new System.Drawing.Size(100, 50);
        this.btnSaveToDB.Text = "Save to DB";
        this.btnSaveToDB.Click += new System.EventHandler(this.btnSaveToDB_Click);

        // 
        // btnLoadFromDB
        // 
        this.btnLoadFromDB.Location = new System.Drawing.Point(120, 10);
        this.btnLoadFromDB.Size = new System.Drawing.Size(100, 50);
        this.btnLoadFromDB.Text = "Load from DB";
        this.btnLoadFromDB.Click += new System.EventHandler(this.btnLoadFromDB_Click);

        // 
        // listBox
        // 
        this.listBox.Location = new System.Drawing.Point(10, 70);
        this.listBox.Size = new System.Drawing.Size(210, 120);

        // 
        // DBForm
        // 
        this.ClientSize = new System.Drawing.Size(230, 200);
        this.Controls.Add(this.btnSaveToDB);
        this.Controls.Add(this.btnLoadFromDB);
        this.Controls.Add(this.listBox);
    }

    private void btnSaveToDB_Click(object sender, EventArgs e)
    {
        // Преобразуем матрицу в строку
        string matrixAsString = ""; // Замените это на вашу матрицу, преобразованную в строку

        // Сохраняем строку в базе данных
        string sql = "INSERT INTO matrices (matrix) VALUES (@matrix)";
        SQLiteCommand command = new SQLiteCommand(sql, connection);
        command.Parameters.AddWithValue("@matrix", matrixAsString);
        command.ExecuteNonQuery();

        // Обновляем список матриц
        LoadMatrixList();
    }

    private void btnLoadFromDB_Click(object sender, EventArgs e)
    {
        // Получаем выбранную матрицу из списка
        string selectedMatrixId = listBox.SelectedItem.ToString();

        // Загружаем строку матрицы из базы данных
        string sql = "SELECT matrix FROM matrices WHERE id = @id";
        SQLiteCommand command = new SQLiteCommand(sql, connection);
        command.Parameters.AddWithValue("@id", selectedMatrixId);
        SQLiteDataReader reader = command.ExecuteReader();

        if (reader.Read())
        {
            string matrixAsString = reader["matrix"].ToString();

            // Преобразуем строку обратно в матрицу
            // Замените это на преобразование строки обратно в матрицу
        }
    }

    private void LoadMatrixList()
    {
        // Загружаем список матриц из базы данных
        string sql = "SELECT id FROM matrices";
        SQLiteCommand command = new SQLiteCommand(sql, connection);
        SQLiteDataReader reader = command.ExecuteReader();

        listBox.Items.Clear();

        while (reader.Read())
        {
            listBox.Items.Add(reader["id"].ToString());
        }
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