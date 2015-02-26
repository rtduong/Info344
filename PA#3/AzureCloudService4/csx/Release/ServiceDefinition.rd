<?xml version="1.0" encoding="utf-8"?>
<serviceModel xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema" name="AzureCloudService4" generation="1" functional="0" release="0" Id="7cb72286-1005-4a2c-966a-f990caef145b" dslVersion="1.2.0.0" xmlns="http://schemas.microsoft.com/dsltools/RDSM">
  <groups>
    <group name="AzureCloudService4Group" generation="1" functional="0" release="0">
      <componentports>
        <inPort name="WebRole1:Endpoint1" protocol="http">
          <inToChannel>
            <lBChannelMoniker name="/AzureCloudService4/AzureCloudService4Group/LB:WebRole1:Endpoint1" />
          </inToChannel>
        </inPort>
      </componentports>
      <settings>
        <aCS name="WebRole1:StoredConnectionString" defaultValue="">
          <maps>
            <mapMoniker name="/AzureCloudService4/AzureCloudService4Group/MapWebRole1:StoredConnectionString" />
          </maps>
        </aCS>
        <aCS name="WebRole1Instances" defaultValue="[1,1,1]">
          <maps>
            <mapMoniker name="/AzureCloudService4/AzureCloudService4Group/MapWebRole1Instances" />
          </maps>
        </aCS>
        <aCS name="WorkerRole1:Setting1" defaultValue="">
          <maps>
            <mapMoniker name="/AzureCloudService4/AzureCloudService4Group/MapWorkerRole1:Setting1" />
          </maps>
        </aCS>
        <aCS name="WorkerRole1:StoredConnectionString" defaultValue="">
          <maps>
            <mapMoniker name="/AzureCloudService4/AzureCloudService4Group/MapWorkerRole1:StoredConnectionString" />
          </maps>
        </aCS>
        <aCS name="WorkerRole1Instances" defaultValue="[1,1,1]">
          <maps>
            <mapMoniker name="/AzureCloudService4/AzureCloudService4Group/MapWorkerRole1Instances" />
          </maps>
        </aCS>
      </settings>
      <channels>
        <lBChannel name="LB:WebRole1:Endpoint1">
          <toPorts>
            <inPortMoniker name="/AzureCloudService4/AzureCloudService4Group/WebRole1/Endpoint1" />
          </toPorts>
        </lBChannel>
      </channels>
      <maps>
        <map name="MapWebRole1:StoredConnectionString" kind="Identity">
          <setting>
            <aCSMoniker name="/AzureCloudService4/AzureCloudService4Group/WebRole1/StoredConnectionString" />
          </setting>
        </map>
        <map name="MapWebRole1Instances" kind="Identity">
          <setting>
            <sCSPolicyIDMoniker name="/AzureCloudService4/AzureCloudService4Group/WebRole1Instances" />
          </setting>
        </map>
        <map name="MapWorkerRole1:Setting1" kind="Identity">
          <setting>
            <aCSMoniker name="/AzureCloudService4/AzureCloudService4Group/WorkerRole1/Setting1" />
          </setting>
        </map>
        <map name="MapWorkerRole1:StoredConnectionString" kind="Identity">
          <setting>
            <aCSMoniker name="/AzureCloudService4/AzureCloudService4Group/WorkerRole1/StoredConnectionString" />
          </setting>
        </map>
        <map name="MapWorkerRole1Instances" kind="Identity">
          <setting>
            <sCSPolicyIDMoniker name="/AzureCloudService4/AzureCloudService4Group/WorkerRole1Instances" />
          </setting>
        </map>
      </maps>
      <components>
        <groupHascomponents>
          <role name="WebRole1" generation="1" functional="0" release="0" software="C:\Users\Raymond\documents\visual studio 2013\Projects\AzureCloudService4\csx\Release\roles\WebRole1" entryPoint="base\x64\WaHostBootstrapper.exe" parameters="base\x64\WaIISHost.exe " memIndex="-1" hostingEnvironment="frontendadmin" hostingEnvironmentVersion="2">
            <componentports>
              <inPort name="Endpoint1" protocol="http" portRanges="80" />
            </componentports>
            <settings>
              <aCS name="StoredConnectionString" defaultValue="" />
              <aCS name="__ModelData" defaultValue="&lt;m role=&quot;WebRole1&quot; xmlns=&quot;urn:azure:m:v1&quot;&gt;&lt;r name=&quot;WebRole1&quot;&gt;&lt;e name=&quot;Endpoint1&quot; /&gt;&lt;/r&gt;&lt;r name=&quot;WorkerRole1&quot; /&gt;&lt;/m&gt;" />
            </settings>
            <resourcereferences>
              <resourceReference name="DiagnosticStore" defaultAmount="[4096,4096,4096]" defaultSticky="true" kind="Directory" />
              <resourceReference name="EventStore" defaultAmount="[1000,1000,1000]" defaultSticky="false" kind="LogStore" />
            </resourcereferences>
          </role>
          <sCSPolicy>
            <sCSPolicyIDMoniker name="/AzureCloudService4/AzureCloudService4Group/WebRole1Instances" />
            <sCSPolicyUpdateDomainMoniker name="/AzureCloudService4/AzureCloudService4Group/WebRole1UpgradeDomains" />
            <sCSPolicyFaultDomainMoniker name="/AzureCloudService4/AzureCloudService4Group/WebRole1FaultDomains" />
          </sCSPolicy>
        </groupHascomponents>
        <groupHascomponents>
          <role name="WorkerRole1" generation="1" functional="0" release="0" software="C:\Users\Raymond\documents\visual studio 2013\Projects\AzureCloudService4\csx\Release\roles\WorkerRole1" entryPoint="base\x64\WaHostBootstrapper.exe" parameters="base\x64\WaWorkerHost.exe " memIndex="-1" hostingEnvironment="consoleroleadmin" hostingEnvironmentVersion="2">
            <settings>
              <aCS name="Setting1" defaultValue="" />
              <aCS name="StoredConnectionString" defaultValue="" />
              <aCS name="__ModelData" defaultValue="&lt;m role=&quot;WorkerRole1&quot; xmlns=&quot;urn:azure:m:v1&quot;&gt;&lt;r name=&quot;WebRole1&quot;&gt;&lt;e name=&quot;Endpoint1&quot; /&gt;&lt;/r&gt;&lt;r name=&quot;WorkerRole1&quot; /&gt;&lt;/m&gt;" />
            </settings>
            <resourcereferences>
              <resourceReference name="DiagnosticStore" defaultAmount="[4096,4096,4096]" defaultSticky="true" kind="Directory" />
              <resourceReference name="EventStore" defaultAmount="[1000,1000,1000]" defaultSticky="false" kind="LogStore" />
            </resourcereferences>
          </role>
          <sCSPolicy>
            <sCSPolicyIDMoniker name="/AzureCloudService4/AzureCloudService4Group/WorkerRole1Instances" />
            <sCSPolicyUpdateDomainMoniker name="/AzureCloudService4/AzureCloudService4Group/WorkerRole1UpgradeDomains" />
            <sCSPolicyFaultDomainMoniker name="/AzureCloudService4/AzureCloudService4Group/WorkerRole1FaultDomains" />
          </sCSPolicy>
        </groupHascomponents>
      </components>
      <sCSPolicy>
        <sCSPolicyUpdateDomain name="WebRole1UpgradeDomains" defaultPolicy="[5,5,5]" />
        <sCSPolicyUpdateDomain name="WorkerRole1UpgradeDomains" defaultPolicy="[5,5,5]" />
        <sCSPolicyFaultDomain name="WebRole1FaultDomains" defaultPolicy="[2,2,2]" />
        <sCSPolicyFaultDomain name="WorkerRole1FaultDomains" defaultPolicy="[2,2,2]" />
        <sCSPolicyID name="WebRole1Instances" defaultPolicy="[1,1,1]" />
        <sCSPolicyID name="WorkerRole1Instances" defaultPolicy="[1,1,1]" />
      </sCSPolicy>
    </group>
  </groups>
  <implements>
    <implementation Id="fdd8d13c-8ae7-4ea9-b121-d7e946b161e0" ref="Microsoft.RedDog.Contract\ServiceContract\AzureCloudService4Contract@ServiceDefinition">
      <interfacereferences>
        <interfaceReference Id="38e159fc-5cb2-4c4d-9e5a-ae1f5012f4c1" ref="Microsoft.RedDog.Contract\Interface\WebRole1:Endpoint1@ServiceDefinition">
          <inPort>
            <inPortMoniker name="/AzureCloudService4/AzureCloudService4Group/WebRole1:Endpoint1" />
          </inPort>
        </interfaceReference>
      </interfacereferences>
    </implementation>
  </implements>
</serviceModel>