For any application there is a concern to read key value pairs from web/app.config. If you have a web application chances are you also need to read write possible key values pairs to the Session from the HttpContext. You can add to this for a app with any decent load probably the ability to also read and write key value pairs to some other persistent store i.e. memcached, database etc. 

To address these different concerns all to with reading and writing kvps, Configoo has a class called 'Configured' that provides a fluent, unified and strongly typed key resolution API to the developer regardless of whether he/she intends to read from the app/web.config or read/write to the Session or any persistent store.

Once you include the package and include the Configoo namespace

	var timeout = A<Configured>.Value.For("connectiontimeout");

will retreive the 'connectiontimeout' property from the app/web.config's appsettings section and assign it to the timeout variable as a string.

	// convert to an int
	var timeout = A<Configured>.Value.For<int>("connectiontimeout");

	// if key not exists provide a default to be used 
	// hereafter against the key value.
	// notice didn't have to explicity cast it to an int.
	// '@default' is just naming the parameter for better readability.
	var timeout = A<Configured>.Value.For("connectiontimeout", @default: 10);

The above code ensures the value 10 is stored in memory against the string key 'connectiontimeout'. It does not write this to disk any where.

Or lets say you have the following class
	
	public class Application {
		public int Id {get;set;}
		public string Language {get;set;}
	}
	
If you wish to populate the properties on the above class and the rest of the app needs it in different places.
	
	var app = new Application
	{
		Id = A<Configured>.Value
			.For<Application, int>(x => x.Id),
		Language = A<Configured>.Value
			.For<Application, string>(x => x.Language)
	}
	
	// stores in memory a key 'Application' with 
	// a reference to the app variable as the value.
	// if a key by that name already exists it will NOT be over written.
	A<Configured>.Value.For<Application>(app);
	
	// will retreive the value of app store above if accessed from any where in the app.
	var a = A<Configured>.Value.For<Application>();
	
Finally to read a connection string

	var northwind = A<Configured>.Value
				.For<ConnectionStringSettings>("Northwind");

will retreive the connection string section named 'Northwind' from the app/web.config.

	
The Configured class uses IGetConfigurationValues interface internally to retreive values into an IDictionary<string, object> when the the first value is requested.

The implementation of the above interface that is injected into the Configured class by Ninject is one that goes to the app/web.config to read all connection string and appsetting values.
	
If you wish to override this behavior you can write your own public class that implements the above interface and Configoo will be happy to automatically detect and use your implementation instead.

Now lets say you wish to use the same API to read and also write values to the SessionState. This would be a very obvious need since it would ensure a consistent api for retreiving values from both the SessionState and the app/web.config.

This can be acheived as follows.
	
	public class UserSession : Configured
	{
		protected UserSession() : base(new GetSessionStateValues()) {}
		
		private class GetSessionStateValues : IGetConfigurationValues
		{
			// implementation to read and 
			// write values from the SessionState
		}
	}
	
Now we can

	// goes to the app/web.config
	var timeout = A<Configured>.Value.For<int>("connectiontimeout");
	
	// goes to the  session state and adds a value if the key does not exist.
	var userid = A<UserSession>.Value.For("username", @default: "afif");
	

	


