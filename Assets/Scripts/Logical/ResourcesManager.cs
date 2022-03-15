using System;
using System.Collections.Generic;
using System.Linq;

namespace Homeworlds.Logical
{
	public class ResourcesManager :IResourcesManager
	{
		const int k_MaxResourceOfAttributes = 3;
		public IBoardManager BoardManager { get; set; }

		public ResourcesManager()
		{ }

		public bool TryRemovePip(Pip i_Attributes)
		{
			int state = (int)BoardManager.Current.Bank;
			int shftCount = getPipCountShiftAmount(i_Attributes);
			int count = (state & (k_MaxResourceOfAttributes << shftCount)) >> shftCount;
			bool attributesAvailable = count > 0;

			if (attributesAvailable)
			{
				updateState(state, --count, shftCount);
			}

			return attributesAvailable;
		}

		public bool TryAddPip(Pip i_Attributes)
		{
			int state = (int)BoardManager.Current.Bank;
			int shftCount = getPipCountShiftAmount(i_Attributes);
			int count = (state & (k_MaxResourceOfAttributes << shftCount)) >> shftCount;
			bool attributesAvailable = count < k_MaxResourceOfAttributes;

			if (attributesAvailable)
			{
				updateState(state, ++count, shftCount);
			}

			return attributesAvailable;
		}

		public void AddPips(Dictionary<Pip, int> i_PipsAttributes)
		{
			int state = (int)BoardManager.Current.Bank;
			validateValues(state, i_PipsAttributes);

			foreach (KeyValuePair<Pip, int> pair in i_PipsAttributes)
			{
				state += pair.Value << getPipCountShiftAmount(pair.Key);
			}

			BoardManager.UpdateResources(new ResourcesState(state));
		}


		private void updateState(int i_OldState, int i_Count, int i_ShiftCount)
		{
			i_OldState &= (int)ResourcesState.Full - (k_MaxResourceOfAttributes << i_ShiftCount);
			int state = i_OldState + (i_Count << i_ShiftCount);
			BoardManager.UpdateResources(new ResourcesState(state));
		}

		public ResourcesState CreateResourcesState(Dictionary<Pip, int> i_Values)
		{
			validateValues(i_Values);
			int state = (int)ResourcesState.Empty;

			foreach (KeyValuePair<Pip, int> pair in i_Values)
			{
				state += pair.Value << getPipCountShiftAmount(pair.Key);
			}

			return new ResourcesState(state);
		}

		private void validateValues(Dictionary<Pip, int> i_Values)
		{
			var outOfRangeValues = i_Values.Values.Where(i => i < 0 || i > k_MaxResourceOfAttributes);
			if (outOfRangeValues.Count() != 0)
			{
				throw new ArgumentOutOfRangeException($"All values must be positive and less than or equal to {k_MaxResourceOfAttributes}! Value was {outOfRangeValues.First()}");
			}

			var undefinedKeys = i_Values.Keys.Where(p => !Enum.IsDefined(typeof(ePipColor), p.Color) || !Enum.IsDefined(typeof(ePipSize), p.Size));
			if (undefinedKeys.Count() != 0)
			{
				throw new ArgumentException($"Found Undefined Color or Size! {undefinedKeys.First()}");
			}
		}

		private void validateValues(int i_State, Dictionary<Pip, int> i_PipsAttributes)
		{
			validateValues(i_PipsAttributes);

			foreach (KeyValuePair<Pip, int> pair in i_PipsAttributes)
			{
				int stateCount = getCurrentCountInternal(i_State, pair.Key);
				if (stateCount + pair.Value > k_MaxResourceOfAttributes)
				{
					throw new ArgumentOutOfRangeException($"Could not add {pair.Value} more of {pair.Key}. State already contains {stateCount} pips!");
				}
				
			}
		}

		private static int getPipCountShiftAmount(Pip i_Attributes)
		{
			return 2 * (int)i_Attributes.Size + 6 * (int)i_Attributes.Color;
		}

		public int GetCurrentCount(Pip i_Attributes)
		{
			return getCurrentCountInternal((int)BoardManager.Current.Bank, i_Attributes);
		}

		private int getCurrentCountInternal(int i_State, Pip i_Attributes)
		{
			int shift = getPipCountShiftAmount(i_Attributes);
			return (i_State & (k_MaxResourceOfAttributes << shift)) >> shift;
		}

		public int GetCurrentCount(ePipColor i_PipColor, ePipSize i_PipSize)
		{
			return GetCurrentCount(new Pip(i_PipColor, i_PipSize));
		}
	}
}