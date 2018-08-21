using UnityEngine;

public static class RendererExtensions {

    public static bool IsVisibleFrom(this Renderer renderer, Camera camera) {

        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(camera);

        return GeometryUtility.TestPlanesAABB(planes, renderer.bounds);
    }
}

public static class SkinnedMeshRendererExtensions {

    public static bool IsVisibleFrom(this SkinnedMeshRenderer renderer, Camera camera) {

        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(camera);

        return GeometryUtility.TestPlanesAABB(planes, renderer.bounds);
    }
}