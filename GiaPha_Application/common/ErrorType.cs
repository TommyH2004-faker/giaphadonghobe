namespace GiaPha_Application.Common
{
    public enum ErrorType
    {
        Validation, 
        NotFound, 
        Conflict,
        Forbidden, 
        Unauthorized,
        Failure,
        NotActivated,
        WrongPassword,
        DatabaseError,
        InternalError,
        InternalServerError 
    }
}
