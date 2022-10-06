using UnityEngine;

public class PostprocessingBlur : MonoBehaviour {
	[SerializeField]
	private Material postprocessMaterial;

	void OnRenderImage(RenderTexture source, RenderTexture destination) {
		var temporaryTexture = RenderTexture.GetTemporary(source.width, source.height);
		Graphics.Blit(source, temporaryTexture, postprocessMaterial, 0);
		Graphics.Blit(temporaryTexture, destination, postprocessMaterial, 1);
		RenderTexture.ReleaseTemporary(temporaryTexture);
	}
}