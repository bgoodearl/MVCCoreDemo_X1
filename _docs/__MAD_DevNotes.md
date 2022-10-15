# MVC Core 3.1 Web App w/AAD Auth - DevNotes

## Dev Notes

<table>
	<tr>
		<th>Date</th>
		<th>Dev</th>
		<th>Notes</th>
	</tr>
	<tr>
		<td>10/15/2022</td><td>bg</td>
		<td>
			Additional conditional code for USE_IDSVR6
            including Home.TokenTest<br/>
            Added branch USE_IDSVR6 with USE_IDSVR6 in project definitions<br/>
            Added API controllers - Identity and WeatherForecast<br/>
            Added view for TokenTest<br/>
            Enabled TokenTest when NOT USE_IDSVR6<br/>
		</td>
	</tr>
	<tr>
		<td>10/13/2022</td><td>bg</td>
		<td>
            Added middleware to ensure removal of auth cookie after signout<br/>
			Made auth cookie name and path configurable for Microsoft.Identity<br/>
		</td>
	</tr>
	<tr>
		<td>10/12/2022</td><td>bg</td>
		<td>
			Switched to Microsoft.Identity.Web<br/>
            Changed UI URLs to start with "~/x2/"
		</td>
	</tr>
	<tr>
		<td>10/11/2022</td><td>bg</td>
		<td>
			Started changes to support use of Identity Server v6<br/>
		</td>
	</tr>
	<tr>
		<td>10/10/2022</td><td>bg</td>
		<td>
			Created web app, 
			added ability to have user-specific appsettings<br/>
			Added Blazor demo w/Counter<br/>
            Added Weather Forecast component<br/>
            Added logging with NLog<br/>
            Added Blazor UserInfoComponent<br/>
		</td>
	</tr>
	<tr>
		<td>.</td><td>.</td>
		<td>
			...
		</td>
	</tr>
</table>
