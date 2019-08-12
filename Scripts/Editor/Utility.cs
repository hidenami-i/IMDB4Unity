using UnityExtensions;

namespace IMDB4Unity.Editor
{
	internal static class Utility
	{
		/// <summary>
		/// Convert to csharp type name.
		/// </summary>
		/// <returns>type</returns>
		/// <param name="dbTypeName">type name on database.</param>
		/// <param name="unsigned">unsinged is true</param>
		public static DataType ConvertToCSharpTypeName(string dbTypeName, bool unsigned) {

			string dbTypeNameToLower = dbTypeName.ToLower();

			switch (dbTypeNameToLower) {
				// -128 to 127 
				case "tinyint":

				// -32768 to 32767
				case "smallint":
				case "short":

				// -2147483648 to 2147483647
				case "integer":

				// -8388608 to 8388607
				case "mediumint":
				{
					//					if (unsigned) {
					//						return "uint";
					//					}

					return DataType.Int;
				}

				// -9223372036854775808 to 9223372036854775807
				case "bigint":
				case "long":
				{
					//					if (unsigned) {
					//						return "ulong";
					//					}

					return DataType.Long;
				}

				// -9,223,372,036,854,775,808～9,223,372,036,854,775,807
				case "float":
				{
					return DataType.Float;
				}

				// -3.402823466E+38～-1.175494351E-38
				case "double":
				{
					return DataType.Double;
				}

				// true/false, 1/0
				case "boolean":
				case "bool":
				{
					return DataType.Bool;
				}

				// 4
				case "varchar(4)":

				// 16
				case "varchar(16)":

				// 32
				case "varchar(32)":

				// 64
				case "varchar(64)":

				// 100
				case "varchar(100)":

				// 128
				case "varchar(128)":

				// 256
				case "varchar(256)":

				// 512
				case "varchar(512)":

				// 1024
				case "varchar(1024)":

				// 2048
				case "varchar(2048)":

				case "string":
				case "json":
				case "jsonarray":

					return DataType.String;

				// 1970-01-01 00:00:01' UTC ～ '2038-01-19 03:14:07' UTC
				case "timestamp":
				case "datetime":
				{
					return DataType.DateTime;
				}
			}

			ExDebug.LogError($"Undefined [{dbTypeName}] type has been detected.");
			return DataType.Undefined;
		}
	}
}