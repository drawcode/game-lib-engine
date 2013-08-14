using System;

public class ContentsConfig {   
        
        // ## content
        // StreamingAsset folders have the content for all data on mobile
        // Create a 'StreamingAssets' folder in the project then set the default folder names here.
        
        // Path will look like 'StreamingAssets/popar/vehicle-shoot/vehicle-shoot-1'  
        // All the data/shared/platform folders in this sample are also required
        // Main app data will be in [version]/shared/data + [version]/shared/data/[packname] within the app folder
        
	public static string contentEndpoint = "http://content1.drawlabs.com/";
	
	// Should be all lower case and dashes since it is used in urls on the S3 dynamic data fetch i.e. 'popar-project-1'
	public static string contentRootFolder = "drawlabs"; // company name or product
	public static string contentAppFolder = "game-drawlabs-template"; // app name or game name
	public static string contentDefaultPackFolder = "game-drawlabs-template-pack-1"; // default app content
	public static string contentVersion = "1.0"; // major content changes with app versions
	public static int contentIncrement = 1; // for minor content changes synced or in an update
	public static string contentApiKey = "85366ecb7429c19839e6900a1cfcedc18342f775";
}