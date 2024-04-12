<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <!-- Identity template copies nodes unchanged -->
  <xsl:template match="@* | node()">
    <xsl:copy>
      <xsl:apply-templates select="@* | node()"/>
    </xsl:copy>
  </xsl:template>

  <!-- Sort PackageVersion elements by Include attribute -->
  <xsl:template match="ItemGroup">
    <xsl:copy>
      <xsl:apply-templates select="@*"/>
      <xsl:apply-templates select="PackageVersion">
        <xsl:sort select="@Include"/>
      </xsl:apply-templates>
    </xsl:copy>
  </xsl:template>
</xsl:stylesheet>
