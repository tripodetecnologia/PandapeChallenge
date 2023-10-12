namespace Challenge.Common
{
    public static class Messages
    {

        #region API Messages

        public const string OK = "OK";        
        public const string RecordExist = "{0} ingresado ya existe en otro registro.";
        public const string RecordNotFound = "Registro no existe.";
        public const string RequestFailed = "No fue posible consumir el servicio.";
        public const string SaveOK = "Información guardada correctamente.";
        public const string DeleteOK = "Se eliminó la información correctamente.";
        public const string NotAuthenticated = "No fue autenticado.";
        public const string UnexpectedError = "Ocurrió un error inesperado.";
        public const string ErrorNameAlreadyExist = "Ya existe un registro con los datos ingresados.";
        public const string ErrorBeginDate = "Existe una experiencia que se traslapa con la fecha de ingreso.";
        public const string ErrorEndDate = "Existe una experiencia que se traslapa con la fecha de retiro.";
        public const string RecordWithRelation = "No fue posible eliminar el registro porque tiene relación con {0}.";
        public const string BeginDateGreaterThanEndDateError = "Fecha de ingreso no puede ser mayor a la fecha de retiro.";
        public const string BirthDateError = "Fecha de nacimiento no valida.";

        #endregion
    }
}
