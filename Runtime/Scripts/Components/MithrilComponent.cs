
/** MithrilComponent.cs
*
*	Created by LIAM WOFFORD of CUBEROOT SOFTWARE, LLC.
*
*	Free to use or modify, with or without creditation,
*	under the Creative Commons 0 License.
*/

#region Includes

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

#endregion

namespace Mithril
{
	#region MithrilComponent

	/// <summary>
	/// Base class for a <see cref="MonoBehaviour"/> with a little extra functionality.
	///</summary>

	public class MithrilComponent : MonoBehaviour
	{
		#region Inners

		#region AssignOnAwakeAttribute

		[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
		protected sealed class AssignOnAwakeAttribute : Attribute
		{
			public Type assignType { get; }

			public AssignOnAwakeAttribute(Type assignType)
			{
				this.assignType = assignType;
			}
			public AssignOnAwakeAttribute()
			{
				assignType = null;
			}
		}

		#endregion

		#endregion
		#region Members

		private const BindingFlags ASSIGN_ON_AWAKE_FIELD_FLAGS = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
#if UNITY_EDITOR
		private bool _isAwake = false;
#endif
		#endregion
		#region Methods

		protected virtual void Init() { }

		protected virtual void OnValidate()
		{
#if UNITY_EDITOR
			if (_isAwake)
				Init();
#endif
		}

		protected virtual void Awake()
		{
			AssignComponentsOnAwake();

			Init();
#if UNITY_EDITOR
			_isAwake = true;
#endif
		}

		private void AssignComponentsOnAwake()
		{
			var fieldsToAssign = GetType().GetFields(ASSIGN_ON_AWAKE_FIELD_FLAGS).Where(i => i.GetCustomAttribute<AssignOnAwakeAttribute>() != null);
			foreach (var iField in fieldsToAssign)
			{
				var iAttribute = iField.GetCustomAttribute<AssignOnAwakeAttribute>();
				iField.SetValue(this, GetAssignOnAwakeValue(iAttribute, iField.FieldType));
			}

			var propertiesToAssign = GetType().GetProperties(ASSIGN_ON_AWAKE_FIELD_FLAGS).Where(i => i.GetCustomAttribute<AssignOnAwakeAttribute>() != null);
			foreach (var iProperty in propertiesToAssign)
			{
				var iAttribute = iProperty.GetCustomAttribute<AssignOnAwakeAttribute>();
				iProperty.SetValue(this, GetAssignOnAwakeValue(iAttribute, iProperty.PropertyType));
			}
		}

		private Component GetAssignOnAwakeValue(AssignOnAwakeAttribute attribute, Type verifyType)
		{
			var assignType = attribute.assignType ?? verifyType;

			if (!verifyType.IsAssignableFrom(assignType))
				throw new InvalidCastException($"AssignOnAwake Type ({assignType.Name}) must be of type ({verifyType.Name}).");

			return GetComponent(assignType) ?? throw new NullReferenceException($"AssignOnAwake: Null reference found for type ({assignType.Name})");
		}

		#endregion
	}

	#endregion
}