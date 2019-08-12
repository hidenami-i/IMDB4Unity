using System;
using System.Collections.Generic;
using System.Reflection;

namespace IMDB4Unity.Editor
{
	public static class DatabaseReflection
	{
		public static void InsertAll(Type repositoryType, object instance, IList<Dictionary<string, object>> values) {
			MethodInfo testMethod =
				repositoryType.GetMethod("InsertAll", new[] {typeof(IList<Dictionary<string, object>>)});
			testMethod.Invoke(instance, new object[] {values});
		}

		public static void LogAllEntity(Type repositoryType, object instance) {
			MethodInfo testMethod = repositoryType.GetMethod("LogAllEntity");
			testMethod.Invoke(instance, new object[] { });
		}
	}
}