
using UnityEngine;

public static class RenderExtensions
{
	public static bool IsVisibleFrom(this Collider2D renderer, Camera camera)
	{
		Plane[] planes = GeometryUtility.CalculateFrustumPlanes(camera); //Devuelve los 6 planos que conforman el frustrum de la Camera(up, down, left, right, far, near)
		return GeometryUtility.TestPlanesAABB(planes, renderer.bounds); //Devuelve TRUE si los BOUNDS estan dentro o interseccionando con los plano.
																		//Bounds is used by Collider.bounds, Mesh.bounds and Renderer.bounds.
	}
}
