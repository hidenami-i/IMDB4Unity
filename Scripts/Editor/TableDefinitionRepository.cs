#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using DAO4Unity;
using UnityEngine;

namespace IMDB4Unity.Editor
{
	[Serializable]
	public class TableDefinitionRepository : RepositoryBase<TableDefinitionEntity, TableDefinitionRepository>
	{
		[SerializeField] private List<TableDefinitionEntity> tableDefinition = new List<TableDefinitionEntity>();

		protected override List<TableDefinitionEntity> EntityList => tableDefinition;
		
		public List<TableDefinitionEntity> FindAllMaster() {
			return FindAllBy(x => x.SchemaType.IsMaster());
		}

		public List<TableDefinitionEntity> FindAllUser() {
			return FindAllBy(x => x.SchemaType.IsUser());
		}

		public List<TableDefinitionEntity> FindAllLocal() {
			return FindAllBy(x => x.SchemaType.IsLocal());
		}

		public List<TableDefinitionEntity> FindAllEnumerations() {
			return FindAllBy(x => x.SchemaType.IsEnumeration());
		}
	}
}

#endif