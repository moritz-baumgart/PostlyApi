namespace PostlyApi.Models.DTOs
{
    /// <summary>
    /// Represents the result of an SQL Query execution. HasResult is true if Columns and Result is present and false if AffectedRows is present.
    /// </summary>
    public class DatabaseOperationDTO
    {
        public bool HasResult { get; set; }
        public int? AffectedRows { get; set; }
        public IEnumerable<string>? Columns { get; set; }
        public IEnumerable<IEnumerable<string>>? Result { get; set; }

        public DatabaseOperationDTO(int? affectedRows, IEnumerable<string>? columns, IEnumerable<IEnumerable<string>>? result)
        {
            if (affectedRows == null && columns != null && result != null)
            {
                HasResult = true;
            }
            else if (affectedRows != null && columns == null && result == null)
            {
                HasResult = false;
            }
            else
            {
                throw new ArgumentException("Only affectedRows or result can have a value!");
            }
            AffectedRows = affectedRows;
            Columns = columns;
            Result = result;
        }
    }
}
