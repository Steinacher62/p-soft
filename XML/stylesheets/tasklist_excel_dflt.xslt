﻿<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="2.0"
    xmlns="urn:schemas-microsoft-com:office:spreadsheet"
    xmlns:o="urn:schemas-microsoft-com:office:office"
    xmlns:x="urn:schemas-microsoft-com:office:excel"
    xmlns:ss="urn:schemas-microsoft-com:office:spreadsheet"
    xmlns:html="http://www.w3.org/TR/REC-html40"
    xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt" 
    xmlns:user="urn:my-scripts"
    exclude-result-prefixes="msxsl">

    <xsl:output method="xml"/>

    <xsl:key name='color' match='suggestion' use='@color'/>
    <xsl:variable name="stylenrstart" select="21"/>
    <xsl:variable name="height" select="30"/>
    
    <msxsl:script language="C#" implements-prefix="user">
        <![CDATA[
     public System.Collections.Hashtable hash = new System.Collections.Hashtable();
     
     public void add(string key, string value){
       hash.Add(key, value);
     }
     
     public string get(string key){
       return (string) hash[key];
     }
      ]]>
    </msxsl:script>


    <xsl:template match="/suggestions" priority="3">
        <xsl:variable name="suggestionname" select="@name"/>

        <xsl:element name="Workbook">
            <!--
            Define Styles
            -->
            <xsl:call-template name="styles">
                <xsl:with-param name="suggestions" select="." />
            </xsl:call-template>

            <xsl:element name="Worksheet">
                <xsl:attribute name="ss:Name">
                    <xsl:value-of select="$suggestionname"/>
                </xsl:attribute>
                <xsl:element name="Table">
                    <xsl:element name="Column">
                        <xsl:attribute name="ss:Width">90</xsl:attribute>
                    </xsl:element>
                    <!-- Title -->
                    <xsl:element name="Row">
                        <xsl:element name="Cell">
                            <xsl:attribute name="ss:StyleID">
                                <xsl:value-of select="concat('s',$stylenrstart)"/>
                            </xsl:attribute>
                            <xsl:element name="Data">
                                <xsl:attribute name="ss:Type">String</xsl:attribute>
                                <xsl:value-of select="$suggestionname"/>
                            </xsl:element>
                        </xsl:element>
                    </xsl:element>
                    <!-- Newline -->
                    <xsl:element name="Row"></xsl:element>

                    <xsl:for-each select="./author">
                        <xsl:element name="Row">
                            <xsl:attribute name="ss:Height">
                                <xsl:value-of select="$height"/>
                            </xsl:attribute>
                            <xsl:call-template name="author">
                                <xsl:with-param name="suggestions" select="." />
                            </xsl:call-template>
                        </xsl:element>
                    </xsl:for-each>
                </xsl:element>
            </xsl:element>
        </xsl:element>
    </xsl:template>

    <!-- Template Style -->
    <xsl:template name="styles">
        <xsl:element name="Styles">
            <xsl:element name="Style">
                <xsl:attribute name="ss:ID">
                    <xsl:value-of select="concat('s',$stylenrstart)"/>
                </xsl:attribute>
                <xsl:element name="Font">
                    <xsl:attribute name="ss:Bold">1</xsl:attribute>
                </xsl:element>
            </xsl:element>
            <xsl:for-each select="./author/suggestion[generate-id()=generate-id(key('color',@color)[1])]">
                <xsl:sort select="@color"/>
                <xsl:variable name="colordefinition" select="@color" />
                <xsl:variable name="stylenr" select="$stylenrstart+position()"></xsl:variable>
                <xsl:variable name="stylename" select="concat('s',$stylenr)" />

                <xsl:value-of select="user:add($colordefinition,$stylename)"/>

                <xsl:element name="Style">
                    <xsl:attribute name="ss:ID">
                        <xsl:value-of select="$stylename"/>
                    </xsl:attribute>
                    <xsl:element name="Alignment">
                        <xsl:attribute name="ss:Vertical">Bottom</xsl:attribute>
                        <xsl:attribute name="ss:WrapText">1</xsl:attribute>
                    </xsl:element>
                    <xsl:element name="Interior">
                        <xsl:attribute name="ss:Color">
                            <xsl:value-of select="$colordefinition"/>
                        </xsl:attribute>
                        <xsl:attribute name="ss:Pattern">Solid</xsl:attribute>
                    </xsl:element>
                </xsl:element>
                
            </xsl:for-each>
        </xsl:element>
    </xsl:template>

    <!-- Template Author -->
    <xsl:template name="author">
        <xsl:param name="suggestions" />

        <xsl:element name="Cell">
            <xsl:attribute name="ss:StyleID">
                <xsl:value-of select="concat('s',$stylenrstart)"/>
            </xsl:attribute>
            <xsl:element name="Data">
                <xsl:attribute name="ss:Type">String</xsl:attribute>
                <xsl:value-of select="@name"/>
            </xsl:element>
        </xsl:element>

        <xsl:for-each select="$suggestions/suggestion/.">
            <xsl:element name="Cell">
                <xsl:attribute name="ss:StyleID">
                    <!-- Select Style -->
                    <xsl:variable name="colordefinition" select="@color" />
                    <xsl:value-of select="user:get($colordefinition)"/>
                </xsl:attribute>
                <xsl:element name="Data">
                    <xsl:attribute name="ss:Type">String</xsl:attribute>
                    <xsl:value-of select="self::node()"/>
                </xsl:element>
            </xsl:element>
        </xsl:for-each>
    </xsl:template>
</xsl:stylesheet>
