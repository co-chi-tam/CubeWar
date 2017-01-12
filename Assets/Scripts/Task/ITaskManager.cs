using System;

namespace CubeWar {
	public interface ITaskManager
	{

		void OnRegisterTask (ITask task);
		void OnUnregisterTask (ITask task);
		void RemoveAllTask();
	
	}
}

