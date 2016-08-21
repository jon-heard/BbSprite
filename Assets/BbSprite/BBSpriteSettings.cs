using UnityEngine;

namespace bbSprite
{
    public static class BBSpriteSettings
    {
        // True = an images is automatically optimized when added to a BbSprite
        public static bool AutoOptimizeImages = false;
        // True = BbSprite's transparency system is guessed when image is set
        public static bool AutoGuessTransparency = false;
        // True = BbSprite layout type is guessed when an image is added
        public static bool AutoGuessLayout = false;
        // True = BbSprite AnimationClip type is guessed when layout type is set
        public static bool AutoGuessAnimation = false;
        // If true, all orientation pointers are drawn at runtime
        public static bool OrientPtrAtRun = false;
        // What color the orientation pointer is drawn with
        public static Color OrientPtrColor = Color.blue;
    }
}
