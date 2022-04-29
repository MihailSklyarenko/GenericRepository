using GenericRepository.Enums.Sorting;

namespace GenericRepository.Models.Sorting;

public class SortingParameter
{
    private readonly string _fieldName;
    private readonly SortDirection _direction;

    public string FieldName => _fieldName;

    public SortDirection Direction => _direction;

    public SortingParameter(string fieldName, SortDirection sortDirection)
    {
        _fieldName = fieldName;
        _direction = sortDirection;
    }
}