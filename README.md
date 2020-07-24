# config-transformation

This tools will process all the **.template** files from the target directory and replace the placeholder with values from:
- app.json files 
- Environment variables
- Command line values

```
dotnet config-transformation [Path to directory which contains *.template files] ke11=value1
```

For instance, you have some apps
```
./my-console-app
	/configs
		appSettings.json.template
		connection-strings.json.template
		credentails.json.template
./web-api
	appSettings.json.template
```
The result of the tools execution will be:
```
./my-console-app
	/configs
		appSettings.json
		appSettings.json.template
		connection-strings.json
		connection-strings.json.template
		credentails.json
		credentails.json.template
./web-api
	appSettings.json
	appSettings.json.template
```
Where all the files are filled with the values defined in sources above
