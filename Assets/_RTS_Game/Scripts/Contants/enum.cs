public enum EUnitClass { PLAYER, ENEMY, KING }
public enum EUnitState { IDLE, MOVING, BUILDING, ATTACKING, CHOPPING, MINING, DEAD }
public enum EWorkerTask { NONE, BUILD, CHOP, MINE, RETURN_WOOD_RESOURCE, RETURN_GOLD_RESOURCE, RETURN_CHOPPING, RETURN_MINING }
public enum ECombatTask { NONE, GUARD, FIGHT }
public enum ECombatStance { DEFENSIVE, OFFENSIVE }
public enum EStatusNode { RUNNING, SUCCESS, FAILURE }
public enum EInteractType { BUILD, FIGHT, CHOP }
public enum EAttackTrig { HORIZONTAL_ATK_TRIG, UP_ATK_TRIG, DOWN_ATK_TRIG, UP_DIAGONAL_ATK_TRIG, DOWN_DIAGONAL_ATK_TRIG, ATK_TRIG }
public enum ESpawnState { IDLE, SPAWNING, WAITING, FINISHED }
public enum EAudioPriority { HIGH = 0, MEDIUM = 128, LOW = 256 }