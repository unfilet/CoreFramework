public abstract class tk2dBaseSprite 
{
	/// <summary>
	/// Anchor.
	/// NOTE: The order in this enum is deliberate, to initialize at LowerLeft for backwards compatibility.
	/// This is also the reason it is local here. Other Anchor enums are NOT compatbile. Do not cast.
	/// </summary>
    public enum Anchor
    {
		/// <summary>Lower left</summary>
		LowerLeft,
		/// <summary>Lower center</summary>
		LowerCenter,
		/// <summary>Lower right</summary>
		LowerRight,
		/// <summary>Middle left</summary>
		MiddleLeft,
		/// <summary>Middle center</summary>
		MiddleCenter,
		/// <summary>Middle right</summary>
		MiddleRight,
		/// <summary>Upper left</summary>
		UpperLeft,
		/// <summary>Upper center</summary>
		UpperCenter,
		/// <summary>Upper right</summary>
		UpperRight,
    }

	
}
