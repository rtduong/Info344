﻿<?xml version="1.0" encoding="utf-8"?>
<serviceModel xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema" name="AzureCloudService2" generation="1" functional="0" release="0" Id="7972feef-8524-4e88-87c3-1f0978d140b5" dslVersion="1.2.0.0" xmlns="http://schemas.microsoft.com/dsltools/RDSM">
  <groups>
    <group name="AzureCloudService2Group" generation="1" functional="0" release="0">
      <componentports>
        <inPort name="WebRole1:Endpoint1" protocol="http">
          <inToChannel>
            <lBChannelMoniker name="/AzureCloudService2/AzureCloudService2Group/LB:WebRole1:Endpoint1" />
          </inToChannel>
        </inPort>
      </componentports>
      <settings>
        <aCS name="WebRole1Instances" defaultValue="[1,1,1]">
          <maps>
            <mapMoniker name="/AzureCloudService2/AzureCloudService2Group/MapWebRole1Instances" />
          </maps>
        </aCS>
      </settings>
      <channels>
        <lBChannel name="LB:WebRole1:Endpoint1">
          <toPorts>
            <inPortMoniker name="/AzureCloudService2/AzureCloudService2Group/WebRole1/Endpoint1" />
          </toPorts>
        </lBChannel>
      </channels>
      <maps>
        <map name="MapWebRole1Instances" kind="Identity">
          <setting>
            <sCSPolicyIDMoniker name="/AzureCloudService2/AzureCloudService2Group/WebRole1Instances" />
          </setting>
        </map>
      </maps>
      <components>
        <groupHascomponents>
          <role name="WebRole1" generation="1" functional="0" release="0" software="c:\users\raymond\documents\visual studio 2013\Projects\AzureCloudService2\AzureCloudService2\csx\Release\roles\WebRole1" entryPoint="base\x64\WaHostBootstrapper.exe" parameters="base\x64\WaIISHost.exe " memIndex="-1" hostingEnvironment="frontendadmin" hostingEnvironmentVersion="2">
            <componentports>
              <inPort name="Endpoint1" protocol="http" portRanges="80" />
            </componentports>
            <settings>
              <aCS name="__ModelData" defaultValue="&lt;m role=&quot;WebRole1&quot; xmlns=&quot;urn:azure:m:v1&quot;&gt;&lt;r name=&quot;WebRole1&quot;&gt;&lt;e name=&quot;Endpoint1&quot; /&gt;&lt;/r&gt;&lt;/m&gt;" />
            </settings>
            <resourcereferences>
              <resourceReference name="DiagnosticStore" defaultAmount="[4096,4096,4096]" defaultSticky="true" kind="Directory" />
              <resourceReference name="EventStore" defaultAmount="[1000,1000,1000]" defaultSticky="false" kind="LogStore" />
            </resourcereferences>
          </role>
          <sCSPolicy>
            <sCSPolicyIDMoniker name="/AzureCloudService2/AzureCloudService2Group/WebRole1Instances" />
            <sCSPolicyUpdateDomainMoniker name="/AzureCloudService2/AzureCloudService2Group/WebRole1UpgradeDomains" />
            <sCSPolicyFaultDomainMoniker name="/AzureCloudService2/AzureCloudService2Group/WebRole1FaultDomains" />
          </sCSPolicy>
        </groupHascomponents>
      </components>
      <sCSPolicy>
        <sCSPolicyUpdateDomain name="WebRole1UpgradeDomains" defaultPolicy="[5,5,5]" />
        <sCSPolicyFaultDomain name="WebRole1FaultDomains" defaultPolicy="[2,2,2]" />
        <sCSPolicyID name="WebRole1Instances" defaultPolicy="[1,1,1]" />
      </sCSPolicy>
    </group>
  </groups>
  <implements>
    <implementation Id="d106cd56-7bf5-412d-922c-9e8c7685a7bd" ref="Microsoft.RedDog.Contract\ServiceContract\AzureCloudService2Contract@ServiceDefinition">
      <interfacereferences>
        <interfaceReference Id="eb115d77-d7ca-4775-aefa-a027fb0c974c" ref="Microsoft.RedDog.Contract\Interface\WebRole1:Endpoint1@ServiceDefinition">
          <inPort>
            <inPortMoniker name="/AzureCloudService2/AzureCloudService2Group/WebRole1:Endpoint1" />
          </inPort>
        </interfaceReference>
      </interfacereferences>
    </implementation>
  </implements>
</serviceModel>