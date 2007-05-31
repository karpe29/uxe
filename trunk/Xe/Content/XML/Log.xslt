<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:template match="/">
		<html>
			<body style="background-color:#1D1D1D; color:#FFFFFF;font-family:Consolas,Courier New,Tahoma,Sans-Serif;">
				<h1>Xna5D Log</h1>
				<xsl:for-each select="Xna5D_Log/Message">
					<table border="1" style="border-collapse:collapse;background-color:#3D3D3D;" bordercolor="#000000" cellspacing="0" width="500">
						<th>
							<strong>Message</strong>
						</th>
						<tr>
							<td width="150">
								<strong>Source</strong>
							</td>
							<td>
								<xsl:value-of select="Source"/>
							</td>
						</tr>
						<tr>
							<td width="150">
								<strong>Destination</strong>
							</td>
							<td>
								<xsl:value-of select="Destination"/>
							</td>
						</tr>
						<tr>
							<td width="150">
								<strong>Text</strong>
							</td>
							<td>
								<xsl:value-of select="Text"/>
							</td>
						</tr>
					</table>
					<br/>
				</xsl:for-each>

				<xsl:for-each select="Xna5D_Log/Warning">
					<table border="1" style="border-collapse:collapse;background-color:#3D3D3D;" bordercolor="#000000" cellspacing="0" width="500">
						<th>
							<strong style="color:#FF6600">Warning</strong>
						</th>
						<tr>
							<td width="150">
								<strong>Source</strong>
							</td>
							<td>
								<xsl:value-of select="Source"/>
							</td>
						</tr>
						<tr>
							<td width="150">
								<strong>Destination</strong>
							</td>
							<td>
								<xsl:value-of select="Destination"/>
							</td>
						</tr>
						<tr>
							<td width="150">
								<strong>Message</strong>
							</td>
							<td>
								<xsl:value-of select="Text"/>
							</td>
						</tr>
					</table>
					<br/>
				</xsl:for-each>

				<xsl:for-each select="Xna5D_Log/Error">
					<table border="1" style="border-collapse:collapse;background-color:#3D3D3D;" bordercolor="#000000" cellspacing="0" width="500">
						<th>
							<strong style="color:#FF0000">Error</strong>
						</th>
						<tr>
							<td width="150">
								<strong>Source</strong>
							</td>
							<td>
								<xsl:value-of select="Source"/>
							</td>
						</tr>
						<tr>
							<td width="150">
								<strong>Destination</strong>
							</td>
							<td>
								<xsl:value-of select="Destination"/>
							</td>
						</tr>
						<tr>
							<td width="150">
								<strong>Exception Type</strong>
							</td>
							<td>
								<xsl:value-of select="ExceptionType"/>
							</td>
						</tr>
						<tr>
							<td width="150">
								<strong>Message</strong>
							</td>
							<td>
								<xsl:value-of select="Text"/>
							</td>
						</tr>
					</table>
					<br/>
				</xsl:for-each>
			</body>
		</html>
	</xsl:template>
</xsl:stylesheet>
