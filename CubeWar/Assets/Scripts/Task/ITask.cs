using System;
using System.Collections;

namespace CubeWar {
	public interface ITask {

		void StartTask();
		void EndTask();

		bool OnTask();
		float OnTaskProcess();
		string GetTaskName();
	
	}
}

