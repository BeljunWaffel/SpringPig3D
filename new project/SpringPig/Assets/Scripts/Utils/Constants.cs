public class Constants
{
    // Energy
    public static int MAX_ENERGY = 10;              // How much energy the player is allowed to store.
    public static int MIN_ENERGY = 0;              // How much energy the player is allowed to store.
    public static int ENERGY_DOWNGRADE = 1;			// How much energy is lost every time you fall.

    // Player Dimensions
    public static float PLAYER_HEIGHT = .5f;
    public static float PLAYER_WIDTH = 1f;
    public static float PLAYER_LENGTH = 1f;

    // Jump
    public static float JUMP_CLEARANCE = .1f;
    public static int MINI_JUMP_HEIGHT = 1;
    
    // Box
    public static float BOX_SPEED = 1f;

    // Pickup Types
    public static readonly string PICKUP_ADDITION = "Add";          // Pickup Effect - Addition

    // Tags
    public static string TAG_PLAYER = "Player";                     // Tag used to define the player
    public static string TAG_BOX = "Box";                           // Tag used to define a box
    public static string TAG_BUTTON_PUSHER = "Button_Pusher";       // Tag to define things that can push buttons
    public static string TAG_BURNABLE = "Burnable";                 // Tag to define things that can be effected by Lava
    public static string TAG_FLAG = "Flag";
    public static string TAG_BUTTON = "Button";
    public static string TAG_GATE = "Gate";
    public static string TAG_PLATFORM = "Platform";
    public static string TAG_LAVA = "Lava";
    public static string TAG_PICKUP = "Pickup";
    public static string TAG_WALL = "Wall";
    public static string TAG_OBSTACLE = "Obstacle";
    public static string TAG_ENEMY = "Enemy";

    // Level Creation Constants
    public static float DEFAULT_WALL_WIDTH = .5f;
    public static float DEFAULT_WALL_HEIGHT = 2f;
    public static string GATE_PREFIX = "gate.";
    public static string CUBE_PREFIX = "cube.";
    public static string TOGGLE_BUTTON_PREFIX = "button.T.";
    public static string NO_TOGGLE_BUTTON_PREFIX = "button.NT.";
    public static string BOX_PREFIX = "box.";
    public static string LAVA_PREFIX = "lava.";
    public static string PLATFORM_PREFIX = "plat.";
    public static string PICKUP_PREFIX = "pickup.";

}