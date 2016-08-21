
Shader "Sprites/BbSprites/BlobShadow"
{ 
	Properties
	{
		_Shape ("Shape", 2D) = "gray" { TexGen ObjectLinear }
		_Falloff ("FallOff", 2D) = "white" { TexGen ObjectLinear }
		_Fade ("Fade", Float) = 0.3
	}
	Subshader
	{
		Pass
		{
			ZWrite Off
			Offset -1, -1
			Fog { Color (1, 1, 1) }
			AlphaTest Greater 0
			ColorMask RGB
			Blend DstColor Zero
			SetTexture [_Shape]
			{
					combine texture, ONE - texture
					Matrix [_Projector]
			}
			SetTexture [_Falloff]
			{
					constantColor (1,1,1,0)
					combine previous lerp (texture) constant
					Matrix [_ProjectorClip]
			}
			SetTexture [_Falloff]
			{
					constantColor	([_Fade],[_Fade],[_Fade],0)
					combine previous + constant
			}
		}
	}
}
