namespace Walter.Evaluacion.ApiConsultas.Enums
{
    public enum TipoFormaPago
    {
        Efectivo = 1,
        TDC = 2,
        TDD = 3
    }

    public static class TipoFormaPagoExtensions
    {
        private static readonly Dictionary<int, TipoFormaPago> _codigoToEnum = new()
    {
        { 1, TipoFormaPago.Efectivo },
        { 2, TipoFormaPago.TDC },
        { 3, TipoFormaPago.TDD }
    };

        private static readonly Dictionary<TipoFormaPago, string> _enumToNombre = new()
    {
        { TipoFormaPago.Efectivo, "Efectivo" },
        { TipoFormaPago.TDC, "TDC" },
        { TipoFormaPago.TDD, "TDD" }
    };

        /// <summary>
        /// Convierte un código numérico de forma de pago (1, 2, 3) a su nombre descriptivo
        /// </summary>
        /// <param name="codigoFormaPago">Código numérico de la forma de pago</param>
        /// <returns>Nombre descriptivo de la forma de pago o el código como string si no existe</returns>
        public static string GetNombreFormaPago(this int codigoFormaPago)
        {
            if (_codigoToEnum.TryGetValue(codigoFormaPago, out var forma))
            {
                return _enumToNombre.TryGetValue(forma, out var nombre) ? nombre : codigoFormaPago.ToString();
            }

            return codigoFormaPago.ToString();
        }

        /// <summary>
        /// Valida si un código de forma de pago es válido
        /// </summary>
        public static bool IsValidFormaPago(this int codigoFormaPago)
        {
            return _codigoToEnum.ContainsKey(codigoFormaPago);
        }

        /// <summary>
        /// Obtiene todos los códigos válidos de forma de pago
        /// </summary>
        public static IEnumerable<int> GetValidCodes()
        {
            return _codigoToEnum.Keys;
        }

        /// <summary>
        /// Obtiene todos los nombres válidos de forma de pago
        /// </summary>
        public static IEnumerable<string> GetValidNames()
        {
            return _enumToNombre.Values;
        }
    }
}
