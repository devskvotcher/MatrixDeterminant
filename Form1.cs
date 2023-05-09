using MatrixDeterminant;
using System;
using System.Data;
using System.Data.SQLite;
using System.Windows.Forms;

public partial class Form1 : Form
{
    private DataGridView dataGridView;
    private Button btnCalculate, btnExit, btnCreate;
    private NumericUpDown numUpDownRows;
    private Label lblResult;
    private int matrixRows, matrixColumns;
    private Button btnLoadFromFile;
    private Button btnDBDialog;    
    public Form1()
    {
        InitializeComponent();       
    }

    private void InitializeComponent()
    {
        this.dataGridView = new DataGridView();
        this.btnCalculate = new Button();
        this.btnExit = new Button();
        this.btnCreate = new Button();
        this.numUpDownRows = new NumericUpDown();
        this.lblResult = new Label();

        // 
        // dataGridView
        // 
        this.dataGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        this.dataGridView.Location = new Point(50, 150);
        this.dataGridView.Size = new Size(300, 300);
        this.dataGridView.AllowUserToAddRows = false;
        this.dataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

        // 
        // btnCalculate
        // 
        this.btnCalculate.Location = new Point(400, 50);
        this.btnCalculate.Size = new Size(100, 50);
        this.btnCalculate.Text = "Calculate";
        this.btnCalculate.Click += new System.EventHandler(this.btnCalculate_Click);

        // 
        // btnExit
        // 
        this.btnExit.Location = new Point(400, 150);
        this.btnExit.Size = new Size(100, 50);
        this.btnExit.Text = "Exit";
        this.btnExit.Click += new System.EventHandler(this.btnExit_Click);

        // 
        // btnCreate
        // 
        this.btnCreate.Location = new Point(300, 50);
        this.btnCreate.Size = new Size(100, 50);
        this.btnCreate.Text = "Create Matrix";
        this.btnCreate.Click += new System.EventHandler(this.btnCreate_Click);

        // 
        // numUpDownRows
        // 
        this.numUpDownRows.Location = new Point(50, 50);
        this.numUpDownRows.Minimum = 2;
        this.numUpDownRows.Maximum = 10;
        this.numUpDownRows.Value = 2; // set initial value

        // 
        // lblResult
        // 
        this.lblResult.Location = new Point(400, 350);
        this.lblResult.Size = new Size(100, 50);
        this.lblResult.Text = "";
        // 
        // btnLoadFromFile
        // 
        this.btnLoadFromFile = new Button();
        this.btnLoadFromFile.Location = new Point(200, 50);
        this.btnLoadFromFile.Size = new Size(100, 50);
        this.btnLoadFromFile.Text = "Load from file";
        this.btnLoadFromFile.Click += new System.EventHandler(this.btnLoadFromFile_Click);
        // btnDBDialog
        // 
        this.btnDBDialog = new Button();
        this.btnDBDialog.Location = new Point(500, 50);
        this.btnDBDialog.Size = new Size(100, 50);
        this.btnDBDialog.Text = "DB";
        this.btnDBDialog.Click += new System.EventHandler(this.btnDBDialog_Click);
        // 
        // MainForm
        // 
        this.ClientSize = new Size(600, 500);
        this.Controls.Add(this.dataGridView);
        this.Controls.Add(this.btnCalculate);
        this.Controls.Add(this.btnExit);
        this.Controls.Add(this.btnCreate);
        this.Controls.Add(this.numUpDownRows);
        this.Controls.Add(this.lblResult);
        this.Controls.Add(this.btnLoadFromFile);
        this.Controls.Add(this.btnDBDialog);
    }

    private void btnCalculate_Click(object sender, EventArgs e)
    {
        double[,] matrix = new double[matrixRows, matrixColumns];
        for (int i = 0; i < matrixRows; i++)
        {
            for (int j = 0; j < matrixColumns; j++)
            {
                matrix[i, j] = Convert.ToDouble(dataGridView.Rows[i].Cells[j].Value);
            }
        }

        double determinant = CalculateDeterminant(matrix);
        lblResult.Text = $"Determinant: {determinant}";
    }

    private void btnExit_Click(object sender, EventArgs e)
    {
        this.Close();
    }

    private void btnCreate_Click(object sender, EventArgs e)
    {
        matrixRows = matrixColumns = (int)numUpDownRows.Value;

        // Adjust columns
        while (dataGridView.ColumnCount < matrixColumns)
        {
            dataGridView.Columns.Add(new DataGridViewTextBoxColumn());
        }
        while (dataGridView.ColumnCount > matrixColumns)
        {
            dataGridView.Columns.RemoveAt(dataGridView.ColumnCount - 1);
        }

        // Adjust rows
        while (dataGridView.RowCount < matrixRows)
        {
            dataGridView.Rows.Add();
        }
        while (dataGridView.RowCount > matrixRows)
        {
            dataGridView.Rows.RemoveAt(dataGridView.RowCount - 1);
        }
    }

    private double CalculateDeterminant(double[,] matrix)
    {
        int length = matrix.GetLength(0);

        if (length == 2)
        {
            return matrix[0, 0] * matrix[1, 1] - matrix[0, 1] * matrix[1, 0];
        }

        double result = 0;
        for (int i = 0; i < length; i++)
        {
            result += (i % 2 == 1 ? 1 : -1) * matrix[0, i] *
                      CalculateDeterminant(GetMinor(matrix, 0, i));
        }

        return result;
    }

    private double[,] GetMinor(double[,] matrix, int row, int column)
    {
        int length = matrix.GetLength(0);
        double[,] output = new double[length - 1, length - 1];
        int p = 0;
        for (int i = 0; i < length; i++)
        {
            if (i != row)
            {
                int q = 0;
                for (int j = 0; j < length; j++)
                {
                    if (j != column)
                    {
                        output[p, q] = matrix[i, j];
                        q++;
                    }
                }

                p++;
            }
        }

        return output;
    }
    private void btnLoadFromFile_Click(object sender, EventArgs e)
    {
        OpenFileDialog openFileDialog = new OpenFileDialog();

        if (openFileDialog.ShowDialog() == DialogResult.OK)
        {
            string[] lines = File.ReadAllLines(openFileDialog.FileName);
            matrixRows = matrixColumns = lines.Length;

            // Adjust columns
            while (dataGridView.ColumnCount < matrixColumns)
            {
                dataGridView.Columns.Add(new DataGridViewTextBoxColumn());
            }
            while (dataGridView.ColumnCount > matrixColumns)
            {
                dataGridView.Columns.RemoveAt(dataGridView.ColumnCount - 1);
            }

            // Adjust rows
            while (dataGridView.RowCount < matrixRows)
            {
                dataGridView.Rows.Add();
            }
            while (dataGridView.RowCount > matrixRows)
            {
                dataGridView.Rows.RemoveAt(dataGridView.RowCount - 1);
            }

            for (int i = 0; i < matrixRows; i++)
            {
                string[] cells = lines[i].Split(';');
                for (int j = 0; j < matrixColumns; j++)
                {
                    dataGridView.Rows[i].Cells[j].Value = cells[j];
                }
            }
        }
    }
    private void btnDBDialog_Click(object sender, EventArgs e)
    {
        DBForm dbForm = new DBForm();
        dbForm.ShowDialog();
    }
    
}



