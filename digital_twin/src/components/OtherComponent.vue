<template>
    <div class="container">
        <iframe src="http://localhost:8081/?monitor=http://localhost:8298/1/" frameborder="0"></iframe>
    </div>
</template>

<script>
export default {
    async created() {
        await fetch('http://host.docker.internal:9296/', {
            method: 'POST',
            headers: {
                'Content-Type': 'text/xml',
                'Content-ID': 'xml'
            },
            body: `
            <testset xmlns="http://cpee.org/ns/properties/2.0">
  <executionhandler>ruby</executionhandler>
  <dataelements>
    <hasInitialized>true</hasInitialized>
    <direction>1</direction>
  </dataelements>
  <endpoints>
    <starter>http://abgabe.cs.univie.ac.at:9033/start</starter>
  </endpoints>
  <attributes>
    <info>factoryio</info>
    <modeltype>CPEE</modeltype>
    <theme>preset</theme>
  </attributes>
  <description>
    <description xmlns="http://cpee.org/ns/description/1.0">
      <call id="a1" endpoint="starter">
        <parameters>
          <label>Initialize production</label>
          <method>:post</method>
          <arguments>
            <direction>!data.direction</direction>
          </arguments>
        </parameters>
        <code>
          <prepare/>
          <finalize output="result">data.hasInitialized = result['code']</finalize>
          <update output="result"/>
          <rescue output="result"/>
        </code>
        <annotations>
          <_timing>
            <_timing_weight/>
            <_timing_avg/>
            <explanations/>
          </_timing>
          <_context_data_analysis>
            <probes/>
            <ips/>
          </_context_data_analysis>
          <report>
            <url/>
          </report>
          <_notes>
            <_notes_general/>
          </_notes>
        </annotations>
        <documentation>
          <input/>
          <output/>
          <implementation>
            <description/>
          </implementation>
          <code>
            <description/>
          </code>
        </documentation>
      </call>
      <stop id="a2"/>
    </description>
  </description>
  <transformation>
    <description type="copy"/>
    <dataelements type="none"/>
    <endpoints type="none"/>
  </transformation>
</testset>`
        });
    }
}
</script>

<style scoped>
.container {
    max-width: 1200px;
    height: 100vh;
    margin: 0 auto;
    padding: 0 2rem;
}

iframe {
    width: 100%;
    height: 100%;
}
</style>