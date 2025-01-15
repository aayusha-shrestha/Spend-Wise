using SpendWise.Model;

namespace SpendWise.Services.Interface;

public interface IBalanceService
{
    decimal GetBalance(Guid userId);

    //GetBalance - fetch from db
    //SetBalance - store in db
    //CalculateBalance - perform actual calculation
}


//DB json  record
// userid:
// total:
// inflow: 
// outflow:
