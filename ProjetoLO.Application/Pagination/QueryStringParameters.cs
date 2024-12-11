namespace ProjetoLO.Application.Pagination;

public class QueryStringParameters
{
    const int maxPageSize = 50;
    public int PageNumber { get; set; } = 1;

    private int pageSize = maxPageSize;
    public int PageSize 
    {
        get
        {
            return pageSize;
        }
        set
        {
            if (value > maxPageSize)
            {
                pageSize = maxPageSize;
            }
            else
            {
                pageSize = value;
            }
        }
    }
}
