using System;
using System.Collections;

public class CEnum {

	public enum EClassType: int {
		None 		= 0,
		Beast		= 1,
		Fairy		= 2,
		// Human class
		Human 		= 10,
		// Werewolf class
		WereWolf 	= 20,
		// Vampire class
		Vampire 	= 30
	}

	public enum EItemSlot : int {
		Hand = 0,
		Back = 1,
		Hat = 2,
		Necklet = 3,
		Inventory = 4
	}

	public enum EObjectType: byte {
		None 		= 0,
		Player		= 1,
		Enemy	 	= 2,
		Object		= 3,
		Skill		= 4, 
		Plant		= 5, 
		Rock		= 6,
		Item		= 7,
		User 		= 100
	}

	public enum EAnimation : int {
		Idle 		= 0,
		Action_1 	= 10,
		Action_2 	= 11,
		Action_3 	= 12,
		Action_4 	= 13,
		Action_5 	= 14,
		Action_6 	= 15,
		Action_7 	= 16,
		Action_8 	= 17,
		Action_9 	= 18,
		Action_10 	= 19,
		Move 		= 20,
		Death 		= 100
	}

	public enum EElementType : int  {
		None 		= 0,
		Neutral		= 1,
		Poison 		= 2,
		Fire		= 3,
		Ice			= 4,
		Earth		= 5,
		Wind		= 6,
		Light		= 7,
		Dark 		= 8,
		Grass 		= 9,
		Pure 		= 100
	}

	public enum EStatusType : int  {
		None 		= 0,
		Health 		= 1,
		Sanity		= 2,
		Hunger		= 3
	}

	public enum EEnviromentType : int {
		None 		= 0,
		Nature 		= 1,
		Magmar 		= 2,
		Ice 		= 3,
		Desert 		= 4,
		Marsh 		= 5
	}

	public enum EItemType: byte {
		None = 0,
		Common = 1,
		Uncommon = 2,
	}

	public enum EGameMode: byte {
		Offline 	= 0,
		MultiPlayer = 1
	}

}
