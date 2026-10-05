using UnityEngine;

public static class RendererFrustum {

    // GeometryUtility.CalculateFrustumPlanes(Camera) allocates a NEW Plane[6] on every call, and
    // these tests run per renderer, per object, per frame (off-screen indicators and actor
    // shadows both sit on that path). The array-filling overload writes into a caller-supplied
    // buffer instead, so the whole test becomes allocation free.
    //
    // Static and shared: the planes are only read inside the TestPlanesAABB call that follows,
    // never stored, and Unity's main loop is single threaded. Anything calling this off the main
    // thread must pass its own buffer to the explicit overload below.
    private static readonly Plane[] planesShared = new Plane[6];

    public static bool IsVisibleFrom(Renderer renderer, Camera camera, Plane[] planes) {

        if (renderer == null || camera == null || planes == null || planes.Length < 6) {
            return false;
        }

        GeometryUtility.CalculateFrustumPlanes(camera, planes);

        return GeometryUtility.TestPlanesAABB(planes, renderer.bounds);
    }

    public static bool IsVisibleFrom(Renderer renderer, Camera camera) {
        return IsVisibleFrom(renderer, camera, planesShared);
    }
}

public static class RendererExtensions {

    public static bool IsVisibleFrom(this Renderer renderer, Camera camera) {
        return RendererFrustum.IsVisibleFrom(renderer, camera);
    }

    // For callers that batch many tests against ONE camera: calculate the planes once and hand
    // the same buffer in, rather than recalculating them per renderer.
    public static bool IsVisibleFrom(this Renderer renderer, Camera camera, Plane[] planes) {
        return RendererFrustum.IsVisibleFrom(renderer, camera, planes);
    }
}

public static class SkinnedMeshRendererExtensions {

    public static bool IsVisibleFrom(this SkinnedMeshRenderer renderer, Camera camera) {
        return RendererFrustum.IsVisibleFrom(renderer, camera);
    }

    public static bool IsVisibleFrom(this SkinnedMeshRenderer renderer, Camera camera, Plane[] planes) {
        return RendererFrustum.IsVisibleFrom(renderer, camera, planes);
    }
}
