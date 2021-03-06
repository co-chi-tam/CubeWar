﻿using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using LogTest;

namespace CubeWar {
	public class CLog {

		// Debug.Log(string)
		public static void Debug(string text) {
			if (UnityEngine.Debug.isDebugBuild) {
				CLogObject.Instance.ShowLog (CLogObject.ELogType.Debug, GetCallerName(), text);
			}
#if UNITY_EDITOR
			UnityEngine.Debug.Log (text);
#endif
		}

		// Debug.Log(string , object[])
		public static void Debug(string format, params object[] args) {
			if (UnityEngine.Debug.isDebugBuild) {
				CLogObject.Instance.ShowLogFormat (CLogObject.ELogType.Debug, format, GetCallerName (), args);
			}
#if UNITY_EDITOR
			UnityEngine.Debug.LogFormat (format, args);
#endif
		}

		// Debug.LogWarning(string)
		public static void Warning(string text) {
			if (UnityEngine.Debug.isDebugBuild) {
				CLogObject.Instance.ShowLog (CLogObject.ELogType.Warning, GetCallerName (), text);
			}
#if UNITY_EDITOR
			UnityEngine.Debug.LogWarning (text);
#endif
		}

		// Debug.LogWarning(string , object[])
		public static void Warning(string format, params object[] args) {
			if (UnityEngine.Debug.isDebugBuild) {
				CLogObject.Instance.ShowLogFormat (CLogObject.ELogType.Warning, format, GetCallerName (), args);
			}
#if UNITY_EDITOR
			UnityEngine.Debug.LogWarningFormat (format, args);
#endif
		}

		// Debug.LogError(string)
		public static void Error(string text) {
			if (UnityEngine.Debug.isDebugBuild) {
				CLogObject.Instance.ShowLog (CLogObject.ELogType.Error, GetCallerName (), text);
			}
#if UNITY_EDITOR
			UnityEngine.Debug.LogError (text);
#endif
		}

		// Debug.LogError(string , object[])
		public static void Error(string format, params object[] args) {
			if (UnityEngine.Debug.isDebugBuild) {
				CLogObject.Instance.ShowLogFormat (CLogObject.ELogType.Error, format, GetCallerName (), args);
			}
#if UNITY_EDITOR
			UnityEngine.Debug.LogErrorFormat (format, args);
#endif
		}

		// Get method called
		private static string GetCallerName() {
			try {
				System.Diagnostics.StackTrace stack = new System.Diagnostics.StackTrace(true);
				if (stack.FrameCount < 3) {
					return "Unknown";
				}
				var type = stack.GetFrame(2).GetMethod().ReflectedType;
				var name = stack.GetFrame(2).GetMethod().Name;
				var line = stack.GetFrame(2).GetFileLineNumber ();
				return type + ":" + name + "." + line;
			} catch (Exception) {
				return "Unknown";
			}
		}

	}
}

