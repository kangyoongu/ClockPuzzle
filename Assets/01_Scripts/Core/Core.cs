using UnityEngine;

public static class Core
{
    public static Vector3 GetInCircleRandomVec(float radius)
    {
        Vector2 random = Random.insideUnitCircle;
        return new Vector3(random.x, 0f, random.y) * (radius * 0.5f);
    }
    public static User ToggleUser(User user)
    {
        return (User)((int)user * -1);
    }
    public static Vector3 CubePos(Vector3 pos)
    {
        return pos + Vector3.up * 3f + GetInCircleRandomVec(0.9f);
    }
    public static float Remap(float value, float fromMin, float fromMax, float toMin, float toMax)
    {
        return toMin + (value - fromMin) * (toMax - toMin) / (fromMax - fromMin);
    }
} 