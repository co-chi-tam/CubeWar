﻿using UnityEngine;
using System;
using System.Collections;

namespace CubeWar {
	public interface ICommunicate {

		void ShowChat(string value);
		void SetChat (string value);
		string GetChat();

		void ShowEmotion(string value);
		void SetEmotion (string value);
		string GetEmotion();
	
	}
}

