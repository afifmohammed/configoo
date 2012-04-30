If you wish to access values from the <appconfig> section of your config or retreive a connection string 
	- use the AppConfig class
	- when its constructred, gets a Lazy Dictionary injected that when evaluated 
	  will get populated with Appconfig values and Connection strings
	- recommended usage is to instantiate it as a global and re-use.
	- if using containers register in container as singleton.
	- AppConfig provides ability to provide default values for keys on request.
	  If the key doesn't exist it will be added to an in memory dictionary with the default value.
	  
If you wish to provide overrides on how values are added to an in memory dictionary and how they are retreived
	- Inherit the HasLookupValues class
	- Override the Get<TValue>(string key, TValue @default) method to provide custom behaviour for retreival and adding defaults
	- Override the IsNotNull<TValue>(TValue value) method to provide custom behaviour on how to evaluate if provided value is not null.
	- Is helpful when
		- when you wish to talk to another set of configuration values besides the AppConfig
		- want the same unified interface that the AppConfig class provides.
		- are happy to store missing keys with default values (when provided) in an in memory dictionary.

If you also wish to override the fact that default values are stored in an in memory dictionary
	- Implement the IHaveLookupValues interface.
	- useful when you wish to
		- say retreive values from the SessionState, or say Memcached.
		- probably persist default values and not have them in memory.


	
	