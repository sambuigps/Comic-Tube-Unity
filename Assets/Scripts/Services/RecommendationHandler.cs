using System;

public class RecommendationResponse
{
    public Comic[] popular_comics;
    public Comic[] latest_comics;
    public Comic[] random_comics;
}

public static class RecommendationHandler
{
    public static void Fetch(Action<ApiResponse<RecommendationResponse>> callback = null)
    {
        ApiHandler.Send<object, RecommendationResponse>(
            SO.env.RecommendationEndpoint,
            "GET",
            null,
            callback
        );
    }
}
