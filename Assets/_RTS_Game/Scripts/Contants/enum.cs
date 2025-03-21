public enum EUnitClass { PLAYER, ENEMY }
public enum EUnitState { IDLE, MOVING, BUILDING, ATTACKING, CHOPPING, DEAD }
public enum EWorkerTask { NONE, BUILD, CHOP, RETURN_RESOURCE, RETURN_CHOPPING }
public enum ECombatTask { NONE, GUARD, FIGHT }
public enum ECombatStance { DEFENSIVE, OFFENSIVE }
public enum EStatusNode { RUNNING, SUCCESS, FAILURE }
public enum EInteractType { BUILD, FIGHT, CHOP }
public enum EAttackTrig { HORIZONTAL_ATK_TRIG, UP_ATK_TRIG, DOWN_ATK_TRIG, UP_DIAGONAL_ATK_TRIG, DOWN_DIAGONAL_ATK_TRIG, ATK_TRIG }