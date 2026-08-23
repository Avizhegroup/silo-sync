using Silo.Ui.Gate.DAL;
using Silo.Ui.Gate.DML;

namespace Silo.Ui.Gate;



public partial class frmSearchMachin : Form
{
    List<TruckCross> _ListTruckCrossQuery = new List<TruckCross>();
    ApiBusiness _apiBusiness = new ApiBusiness();
    TruckCross _SelectedTruckCross;
    public frmSearchMachin(TruckCross _selectedTruckCross)
    {
        InitializeComponent();
        _SelectedTruckCross=_selectedTruckCross;
    }

    private void frmSearchMachin_Load(object sender, EventArgs e)
    {
        GetUnexitedCrosses();
    }

    private  async void GetUnexitedCrosses()
    {
        _ListTruckCrossQuery =await _apiBusiness.GetUnexitedCrosses();


        foreach(TruckCross _trc in _ListTruckCrossQuery)
        {
            dataGridView1.Rows.Add(_trc.Id, _trc.DriverName, _trc.plaque);
        }
    }

    private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
    {
        if(e.RowIndex>=0 && e.ColumnIndex==3)
        {
            _SelectedTruckCross.Id=dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString();
            _SelectedTruckCross.DriverName=dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString();
            _SelectedTruckCross.plaque=dataGridView1.Rows[e.RowIndex].Cells[2].Value.ToString();
            this.Close();
        }
    }
}
