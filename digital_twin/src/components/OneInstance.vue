<template>
    <div class="heading">
        <div class="tex">
            <h3 @click="toggleInstance">{{ opened ? "🔽" : "◀️" }} Instanz {{ instanceId }}</h3>
        </div>
        <div class="control">
            <span @click="$emit('delete-instance', instanceId)">❌</span>
        </div>
    </div>
    <iframe v-if="opened" :src="instance" frameborder="0"></iframe>
</template>

<script>
export default {
    async created() {
        // const formData = new URLSearchParams();
        // formData.append('info', 'WfMS for the Digital Twin');
        // const response = await fetch('http://localhost:8298/', {
        //     method: 'POST',
        //     headers: {
        //         'Content-Type': 'application/x-www-form-urlencoded',
        //     },
        //     body: formData
        // });
        const response = await fetch(this.url, {
            method: 'POST',
            headers: this.headers,
            body: this.xml
        });
        const msg = await response.json();
        const instance = msg["CPEE-INSTANCE"];
        this.instance = "http://localhost:8081/?monitor=http://localhost:8298/" + instance + "/";
    },
    props: [
        'instanceId'
    ],
    emits: [
        'delete-instance'
    ],
    data() {
        return {
            url: "http://localhost:9296/xml?behavior=wait_running",
            headers: {
                'Content-Type': 'text/xml',
                'Content-ID': 'xml'
            },
            opened: true,
            instance: null,
            xml: `
            <testset xmlns="http://cpee.org/ns/properties/2.0">
            <executionhandler>ruby</executionhandler>
            <dataelements>
                <id/>
                <palletID/>
                <quality/>
                <mirrorID/>
                <product>schrank</product>
                <doorType>default</doorType>
                <mirrorShape>rund</mirrorShape>
                <tableStyle/>
                <additionalEquipment>true</additionalEquipment>
                <maxHeight>7.5</maxHeight>
                <maxWeight>3.0</maxWeight>
                <height>5</height>
                <weight>2</weight>
                <express>true</express>
                <box/>
                <logisticOption/>
                <machine/>
            </dataelements>
            <endpoints>
                <starter>http://host.docker.internal:9033/start</starter>
                <timeout>http://gruppe.wst.univie.ac.at/~mangler/services/timeout.php</timeout>
                <init>http://host.docker.internal:9033/start/init</init>
                <complete>http://host.docker.internal:9033/start/complete</complete>
                <task>http://host.docker.internal:9033/start/task</task>
                <determineQuality>http://host.docker.internal:9033/start/determineQuality</determineQuality>
                <logisticOption>http://host.docker.internal:9033/start/logisticOption</logisticOption>
                <pack>http://host.docker.internal:9033/start/pack</pack>
                <modernTable>http://host.docker.internal:9033/start/modernTable</modernTable>
                <classicTable>http://host.docker.internal:9033/start/classicTable</classicTable>
                <drawers>http://host.docker.internal:9033/start/drawers</drawers>
                <preDrill>http://host.docker.internal:9033/start/preDrill</preDrill>
                <tableLegs>http://host.docker.internal:9033/start/tableLegs</tableLegs>
                <machining>http://host.docker.internal:9033/start/machining</machining>
                <shelves>http://host.docker.internal:9033/start/shelves</shelves>
                <defaultDoor>http://host.docker.internal:9033/start/defaultDoor</defaultDoor>
                <slidingDoor>http://host.docker.internal:9033/start/slidingDoor</slidingDoor>
                <lock>http://host.docker.internal:9033/start/lock</lock>
                <assemble>http://host.docker.internal:9033/start/assemble</assemble>
                <extras>http://host.docker.internal:9033/start/extras</extras>
                <angular>http://host.docker.internal:9033/start/angularMirror</angular>
                <circular>http://host.docker.internal:9033/start/circularMirror</circular>
                <mirrorMaterial>http://host.docker.internal:9033/start/mirrorMaterial</mirrorMaterial>
                <improve>http://host.docker.internal:9033/start/improve</improve>
                <equipment>http://host.docker.internal:9033/start/equipment</equipment>
            </endpoints>
            <attributes>
                <info>xx</info>
                <modeltype>CPEE</modeltype>
                <theme>preset</theme>
            </attributes>
            <description>
                <description xmlns="http://cpee.org/ns/description/1.0">
                <call id="a36" endpoint="starter">
                    <parameters>
                    <label>Material vorbereiten</label>
                    <method>:post</method>
                    <arguments>
                        <product>!data.product</product>
                        <mirrorShape>!data.mirrorShape</mirrorShape>
                        <doorType>!data.doorType</doorType>
                        <tableStyle>!data.tableStyle</tableStyle>
                        <express>!data.express</express>
                        <additionalEquipment>!data.additionalEquipment</additionalEquipment>
                    </arguments>
                    </parameters>
                    <code>
                    <prepare/>
                    <finalize output="result">data.id = result['id']</finalize>
                    <update output="result"/>
                    <rescue output="result"/>
                    </code>
                    <annotations>
                    <_timing>
                        <_timing_weight/>
                        <_timing_avg/>
                        <explanations/>
                    </_timing>
                    <_shifting>
                        <_shifting_type>Duration</_shifting_type>
                    </_shifting>
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
                <call id="a5" endpoint="init">
                    <parameters>
                    <label>RFID Transponder initialisieren</label>
                    <method>:get</method>
                    <arguments/>
                    </parameters>
                    <code>
                    <prepare/>
                    <finalize output="result">data.palletID = result['reference']</finalize>
                    <update output="result"/>
                    <rescue output="result"/>
                    </code>
                    <annotations>
                    <_timing>
                        <_timing_weight/>
                        <_timing_avg/>
                        <explanations/>
                    </_timing>
                    <_shifting>
                        <_shifting_type>Duration</_shifting_type>
                    </_shifting>
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
                <choose mode="exclusive">
                    <alternative condition="data.product == &quot;schrank&quot;">
                    <call id="a37" endpoint="machining">
                        <parameters>
                        <label>Material zuschneiden mit Maschine B</label>
                        <method>:post</method>
                        <arguments>
                            <reference>!data.palletID</reference>
                            <machine>!data.machine</machine>
                        </arguments>
                        </parameters>
                        <code>
                        <prepare>data.machine = "B";</prepare>
                        <finalize output="result"/>
                        <update output="result"/>
                        <rescue output="result"/>
                        </code>
                        <annotations>
                        <_timing>
                            <_timing_weight/>
                            <_timing_avg/>
                            <explanations/>
                        </_timing>
                        <_shifting>
                            <_shifting_type>Duration</_shifting_type>
                        </_shifting>
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
                    <call id="a2" endpoint="preDrill">
                        <parameters>
                        <label>Löcher vorbohren (Schrank)</label>
                        <method>:post</method>
                        <arguments>
                            <reference>!data.palletID</reference>
                        </arguments>
                        </parameters>
                        <code>
                        <prepare/>
                        <finalize output="result"/>
                        <update output="result"/>
                        <rescue output="result"/>
                        </code>
                        <annotations>
                        <_timing>
                            <_timing_weight/>
                            <_timing_avg/>
                            <explanations/>
                        </_timing>
                        <_shifting>
                            <_shifting_type>Duration</_shifting_type>
                        </_shifting>
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
                    <call id="a6" endpoint="shelves">
                        <parameters>
                        <label>Regale herstellen</label>
                        <method>:post</method>
                        <arguments>
                            <reference>!data.palletID</reference>
                        </arguments>
                        </parameters>
                        <code>
                        <prepare/>
                        <finalize output="result"/>
                        <update output="result"/>
                        <rescue output="result"/>
                        </code>
                        <annotations>
                        <_timing>
                            <_timing_weight/>
                            <_timing_avg/>
                            <explanations/>
                        </_timing>
                        <_shifting>
                            <_shifting_type>Duration</_shifting_type>
                        </_shifting>
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
                    <_probability>
                        <_probability_min/>
                        <_probability_max/>
                        <_probability_avg/>
                    </_probability>
                    <parallel wait="-1" cancel="last">
                        <parallel_branch pass="" local="">
                        <choose mode="exclusive">
                            <alternative condition="data.doorType != &quot;default&quot;">
                            <_probability>
                                <_probability_min/>
                                <_probability_max/>
                                <_probability_avg/>
                            </_probability>
                            <call id="a35" endpoint="slidingDoor">
                                <parameters>
                                <label>Schiebetür herstellen</label>
                                <method>:post</method>
                                <arguments>
                                    <reference>!data.palletID</reference>
                                </arguments>
                                </parameters>
                                <code>
                                <prepare/>
                                <finalize output="result"/>
                                <update output="result"/>
                                <rescue output="result"/>
                                </code>
                                <annotations>
                                <_timing>
                                    <_timing_weight/>
                                    <_timing_avg/>
                                    <explanations/>
                                </_timing>
                                <_shifting>
                                    <_shifting_type>Duration</_shifting_type>
                                </_shifting>
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
                            </alternative>
                            <otherwise>
                            <call id="a41" endpoint="defaultDoor">
                                <parameters>
                                <label>Standardtür herstellen</label>
                                <method>:post</method>
                                <arguments>
                                    <reference>!data.palletID</reference>
                                </arguments>
                                </parameters>
                                <code>
                                <prepare/>
                                <finalize output="result"/>
                                <update output="result"/>
                                <rescue output="result"/>
                                </code>
                                <annotations>
                                <_timing>
                                    <_timing_weight/>
                                    <_timing_avg/>
                                    <explanations/>
                                </_timing>
                                <_shifting>
                                    <_shifting_type>Duration</_shifting_type>
                                </_shifting>
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
                            </otherwise>
                        </choose>
                        <call id="a10" endpoint="lock">
                            <parameters>
                            <label>Türschloss einbauen</label>
                            <method>:post</method>
                            <arguments>
                                <reference>!data.palletID</reference>
                            </arguments>
                            </parameters>
                            <code>
                            <prepare/>
                            <finalize output="result"/>
                            <update output="result"/>
                            <rescue output="result"/>
                            </code>
                            <annotations>
                            <_timing>
                                <_timing_weight/>
                                <_timing_avg/>
                                <explanations/>
                            </_timing>
                            <_shifting>
                                <_shifting_type>Duration</_shifting_type>
                            </_shifting>
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
                        </parallel_branch>
                        <parallel_branch pass="" local="">
                        <call id="a14" endpoint="mirrorMaterial">
                            <parameters>
                            <label>Spiegel zur Bearbeitung vorbereiten</label>
                            <method>:post</method>
                            <arguments>
                                <reference>!data.palletID</reference>
                            </arguments>
                            </parameters>
                            <code>
                            <prepare/>
                            <finalize output="result"/>
                            <update output="result"/>
                            <rescue output="result"/>
                            </code>
                            <annotations>
                            <_timing>
                                <_timing_weight/>
                                <_timing_avg/>
                                <explanations/>
                            </_timing>
                            <_shifting>
                                <_shifting_type>Duration</_shifting_type>
                            </_shifting>
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
                        <choose mode="exclusive">
                            <alternative condition="data.mirrorShape == &quot;rund&quot;">
                            <call id="a4" endpoint="circular">
                                <parameters>
                                <label>Spiegel rund zuschneiden</label>
                                <method>:post</method>
                                <arguments>
                                    <reference>!data.palletID</reference>
                                </arguments>
                                </parameters>
                                <code>
                                <prepare/>
                                <finalize output="result"/>
                                <update output="result"/>
                                <rescue output="result"/>
                                </code>
                                <annotations>
                                <_timing>
                                    <_timing_weight/>
                                    <_timing_avg/>
                                    <explanations/>
                                </_timing>
                                <_shifting>
                                    <_shifting_type>Duration</_shifting_type>
                                </_shifting>
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
                            <_probability>
                                <_probability_min/>
                                <_probability_max/>
                                <_probability_avg/>
                            </_probability>
                            </alternative>
                            <otherwise>
                            <call id="a12" endpoint="angular">
                                <parameters>
                                <label>Spiegel eckig zuschneiden</label>
                                <method>:post</method>
                                <arguments>
                                    <reference>!data.palletID</reference>
                                </arguments>
                                </parameters>
                                <code>
                                <prepare/>
                                <finalize output="result"/>
                                <update output="result"/>
                                <rescue output="result"/>
                                </code>
                                <annotations>
                                <_timing>
                                    <_timing_weight/>
                                    <_timing_avg/>
                                    <explanations/>
                                </_timing>
                                <_shifting>
                                    <_shifting_type>Duration</_shifting_type>
                                </_shifting>
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
                            </otherwise>
                        </choose>
                        </parallel_branch>
                    </parallel>
                    <call id="a11" endpoint="assemble">
                        <parameters>
                        <label>Spiegel auf Tür befestigen</label>
                        <method>:post</method>
                        <arguments>
                            <reference>!data.palletID</reference>
                        </arguments>
                        </parameters>
                        <code>
                        <prepare/>
                        <finalize output="result"/>
                        <update output="result"/>
                        <rescue output="result"/>
                        </code>
                        <annotations>
                        <_timing>
                            <_timing_weight/>
                            <_timing_avg/>
                            <explanations/>
                        </_timing>
                        <_shifting>
                            <_shifting_type>Duration</_shifting_type>
                        </_shifting>
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
                    <call id="a3" endpoint="extras">
                        <parameters>
                        <label>Kleiderhaken und Schrankstange beilegen</label>
                        <method>:post</method>
                        <arguments>
                            <reference>!data.palletID</reference>
                        </arguments>
                        </parameters>
                        <code>
                        <prepare/>
                        <finalize output="result"/>
                        <update output="result"/>
                        <rescue output="result"/>
                        </code>
                        <annotations>
                        <_timing>
                            <_timing_weight/>
                            <_timing_avg/>
                            <explanations/>
                        </_timing>
                        <_shifting>
                            <_shifting_type>Duration</_shifting_type>
                        </_shifting>
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
                    </alternative>
                    <otherwise>
                    <call id="a38" endpoint="machining">
                        <parameters>
                        <label>Material zuschneiden mit Maschine A</label>
                        <method>:post</method>
                        <arguments>
                            <reference>!data.palletID</reference>
                            <machine>!data.machine</machine>
                        </arguments>
                        </parameters>
                        <code>
                        <prepare>data.machine = "A";</prepare>
                        <finalize output="result"/>
                        <update output="result"/>
                        <rescue output="result"/>
                        </code>
                        <annotations>
                        <_timing>
                            <_timing_weight/>
                            <_timing_avg/>
                            <explanations/>
                        </_timing>
                        <_shifting>
                            <_shifting_type>Duration</_shifting_type>
                        </_shifting>
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
                    <call id="a7" endpoint="preDrill">
                        <parameters>
                        <label>Löcher vorbohren (Schreibtisch)</label>
                        <method>:post</method>
                        <arguments>
                            <reference>!data.palletID</reference>
                        </arguments>
                        </parameters>
                        <code>
                        <prepare/>
                        <finalize output="result"/>
                        <update output="result"/>
                        <rescue output="result"/>
                        </code>
                        <annotations>
                        <_timing>
                            <_timing_weight/>
                            <_timing_avg/>
                            <explanations/>
                        </_timing>
                        <_shifting>
                            <_shifting_type>Duration</_shifting_type>
                        </_shifting>
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
                    <call id="a8" endpoint="drawers">
                        <parameters>
                        <label>Schubladen herstellen</label>
                        <method>:post</method>
                        <arguments>
                            <reference>!data.palletID</reference>
                        </arguments>
                        </parameters>
                        <code>
                        <prepare/>
                        <finalize output="result"/>
                        <update output="result"/>
                        <rescue output="result"/>
                        </code>
                        <annotations>
                        <_timing>
                            <_timing_weight/>
                            <_timing_avg/>
                            <explanations/>
                        </_timing>
                        <_shifting>
                            <_shifting_type>Duration</_shifting_type>
                        </_shifting>
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
                    <choose mode="exclusive">
                        <alternative condition="data.tableStyel != &quot;classic&quot;">
                        <call id="a21" endpoint="modernTable">
                            <parameters>
                            <label>Kabeldurchführung einfügen</label>
                            <method>:post</method>
                            <arguments>
                                <reference>!data.palletID</reference>
                            </arguments>
                            </parameters>
                            <code>
                            <prepare/>
                            <finalize output="result"/>
                            <update output="result"/>
                            <rescue output="result"/>
                            </code>
                            <annotations>
                            <_timing>
                                <_timing_weight/>
                                <_timing_avg/>
                                <explanations/>
                            </_timing>
                            <_shifting>
                                <_shifting_type>Duration</_shifting_type>
                            </_shifting>
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
                        <_probability>
                            <_probability_min/>
                            <_probability_max/>
                            <_probability_avg/>
                        </_probability>
                        </alternative>
                        <otherwise>
                        <call id="a22" endpoint="classicTable">
                            <parameters>
                            <label>Tischplatte gravieren</label>
                            <method>:post</method>
                            <arguments>
                                <reference>!data.palletID</reference>
                            </arguments>
                            </parameters>
                            <code>
                            <prepare/>
                            <finalize output="result"/>
                            <update output="result"/>
                            <rescue output="result"/>
                            </code>
                            <annotations>
                            <_timing>
                                <_timing_weight/>
                                <_timing_avg/>
                                <explanations/>
                            </_timing>
                            <_shifting>
                                <_shifting_type>Duration</_shifting_type>
                            </_shifting>
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
                        </otherwise>
                    </choose>
                    <call id="a9" endpoint="tableLegs">
                        <parameters>
                        <label>Tischbeine herstellen</label>
                        <method>:post</method>
                        <arguments>
                            <reference>!data.palletID</reference>
                        </arguments>
                        </parameters>
                        <code>
                        <prepare/>
                        <finalize output="result"/>
                        <update output="result"/>
                        <rescue output="result"/>
                        </code>
                        <annotations>
                        <_timing>
                            <_timing_weight/>
                            <_timing_avg/>
                            <explanations/>
                        </_timing>
                        <_shifting>
                            <_shifting_type>Duration</_shifting_type>
                        </_shifting>
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
                    </otherwise>
                </choose>
                <call id="a24" endpoint="task">
                    <parameters>
                    <label>Oberflächenbehandlung</label>
                    <method>:post</method>
                    <arguments>
                        <reference>!data.palletID</reference>
                    </arguments>
                    </parameters>
                    <code>
                    <prepare/>
                    <finalize output="result"/>
                    <update output="result"/>
                    <rescue output="result"/>
                    </code>
                    <annotations>
                    <_timing>
                        <_timing_weight/>
                        <_timing_avg/>
                        <explanations/>
                    </_timing>
                    <_shifting>
                        <_shifting_type>Duration</_shifting_type>
                    </_shifting>
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
                <call id="a33" endpoint="determineQuality">
                    <parameters>
                    <label>Qualitätskontrolle</label>
                    <method>:post</method>
                    <arguments>
                        <reference>!data.palletID</reference>
                    </arguments>
                    </parameters>
                    <code>
                    <prepare/>
                    <finalize output="result">data.quality = result['quality']</finalize>
                    <update output="result"/>
                    <rescue output="result"/>
                    </code>
                    <annotations>
                    <_timing>
                        <_timing_weight/>
                        <_timing_avg/>
                        <explanations/>
                    </_timing>
                    <_shifting>
                        <_shifting_type>Duration</_shifting_type>
                    </_shifting>
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
                <loop mode="pre_test" condition="!data.quality">
                    <_probability>
                    <_probability_min/>
                    <_probability_max/>
                    <_probability_avg/>
                    </_probability>
                    <call id="a15" endpoint="improve">
                    <parameters>
                        <label>Fehlende Bestandteile beilegen</label>
                        <method>:post</method>
                        <arguments>
                        <reference>!data.palletID</reference>
                        </arguments>
                    </parameters>
                    <code>
                        <prepare/>
                        <finalize output="result"/>
                        <update output="result"/>
                        <rescue output="result"/>
                    </code>
                    <annotations>
                        <_timing>
                        <_timing_weight/>
                        <_timing_avg/>
                        <explanations/>
                        </_timing>
                        <_shifting>
                        <_shifting_type>Duration</_shifting_type>
                        </_shifting>
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
                    <call id="a13" endpoint="determineQuality">
                    <parameters>
                        <label>Erneute Qualitätskontrolle</label>
                        <method>:post</method>
                        <arguments>
                        <reference>!data.palletID</reference>
                        </arguments>
                    </parameters>
                    <code>
                        <prepare/>
                        <finalize output="result">data.quality = result['quality']</finalize>
                        <update output="result"/>
                        <rescue output="result"/>
                    </code>
                    <annotations>
                        <_timing>
                        <_timing_weight/>
                        <_timing_avg/>
                        <explanations/>
                        </_timing>
                        <_shifting>
                        <_shifting_type>Duration</_shifting_type>
                        </_shifting>
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
                </loop>
                <choose mode="exclusive">
                    <alternative condition="data.additionalEquipment">
                    <call id="a16" endpoint="equipment">
                        <parameters>
                        <label>Zusatzausrüstung beilegen</label>
                        <method>:post</method>
                        <arguments>
                            <reference>!data.palletID</reference>
                        </arguments>
                        </parameters>
                        <code>
                        <prepare/>
                        <finalize output="result"/>
                        <update output="result"/>
                        <rescue output="result"/>
                        </code>
                        <annotations>
                        <_timing>
                            <_timing_weight/>
                            <_timing_avg/>
                            <explanations/>
                        </_timing>
                        <_shifting>
                            <_shifting_type>Duration</_shifting_type>
                        </_shifting>
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
                    <_probability>
                        <_probability_min/>
                        <_probability_max/>
                        <_probability_avg/>
                    </_probability>
                    </alternative>
                    <otherwise/>
                </choose>
                <stop id="a17"/>
                <manipulate id="a19" label="Produkt mittels Material ermitteln"/>
                <choose mode="exclusive">
                    <alternative condition="data.product == &quot;schreibtisch&quot; and !data.additionalEquipment">
                    <call id="a1" endpoint="pack">
                        <parameters>
                        <label>In Box S verpacken</label>
                        <method>:post</method>
                        <arguments>
                            <reference>!data.palletID</reference>
                            <box>!data.box</box>
                        </arguments>
                        </parameters>
                        <code>
                        <prepare>data.box = "S";</prepare>
                        <finalize output="result"/>
                        <update output="result"/>
                        <rescue output="result"/>
                        </code>
                        <annotations>
                        <_timing>
                            <_timing_weight/>
                            <_timing_avg/>
                            <explanations/>
                        </_timing>
                        <_shifting>
                            <_shifting_type>Duration</_shifting_type>
                        </_shifting>
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
                    <_probability>
                        <_probability_min/>
                        <_probability_max/>
                        <_probability_avg/>
                    </_probability>
                    </alternative>
                    <alternative condition="(data.product == &quot;schreibtisch&quot; and data.additionalEquipment) or (data.product == &quot;schrank&quot; and !data.additionalEquipment)">
                    <call id="a39" endpoint="pack">
                        <parameters>
                        <label>In Box M verpacken</label>
                        <method>:post</method>
                        <arguments>
                            <reference>!data.palletID</reference>
                            <box>!data.box</box>
                        </arguments>
                        </parameters>
                        <code>
                        <prepare>data.box = "M";</prepare>
                        <finalize output="result"/>
                        <update output="result"/>
                        <rescue output="result"/>
                        </code>
                        <annotations>
                        <_timing>
                            <_timing_weight/>
                            <_timing_avg/>
                            <explanations/>
                        </_timing>
                        <_shifting>
                            <_shifting_type>Duration</_shifting_type>
                        </_shifting>
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
                    <_probability>
                        <_probability_min/>
                        <_probability_max/>
                        <_probability_avg/>
                    </_probability>
                    </alternative>
                    <otherwise>
                    <call id="a40" endpoint="pack">
                        <parameters>
                        <label>In Box L verpacken</label>
                        <method>:post</method>
                        <arguments>
                            <reference>!data.palletID</reference>
                            <box>!data.box</box>
                        </arguments>
                        </parameters>
                        <code>
                        <prepare>data.box = "L";</prepare>
                        <finalize output="result"/>
                        <update output="result"/>
                        <rescue output="result"/>
                        </code>
                        <annotations>
                        <_timing>
                            <_timing_weight/>
                            <_timing_avg/>
                            <explanations/>
                        </_timing>
                        <_shifting>
                            <_shifting_type>Duration</_shifting_type>
                        </_shifting>
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
                    </otherwise>
                </choose>
                <manipulate id="a20" label="Pakethöhe messen"/>
                <manipulate id="a26" label="Paket wiegen"/>
                <choose mode="exclusive">
                    <alternative condition="(data.weight &lt; data.maxWeight) and (data.height &lt; data.maxHeight) and !data.express">
                    <manipulate id="a32" label="Lieferschein für kleines Paket drucken"/>
                    <call id="a28" endpoint="logisticOption">
                        <parameters>
                        <label>Lieferschein für kleines Paket drucken</label>
                        <method>:post</method>
                        <arguments>
                            <reference>!data.palletID</reference>
                            <logisticOption>!data.logisticOption</logisticOption>
                        </arguments>
                        </parameters>
                        <code>
                        <prepare>data.logisticOption = "Standard";</prepare>
                        <finalize output="result"/>
                        <update output="result"/>
                        <rescue output="result"/>
                        </code>
                        <annotations>
                        <_timing>
                            <_timing_weight/>
                            <_timing_avg/>
                            <explanations/>
                        </_timing>
                        <_shifting>
                            <_shifting_type>Duration</_shifting_type>
                        </_shifting>
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
                    <_probability>
                        <_probability_min/>
                        <_probability_max/>
                        <_probability_avg/>
                    </_probability>
                    </alternative>
                    <alternative condition="(data.weight &gt;= data.maxWeight) or(data.height &gt;= data.maxHeight) and !data.express">
                    <_probability>
                        <_probability_min/>
                        <_probability_max/>
                        <_probability_avg/>
                    </_probability>
                    <call id="a29" endpoint="logisticOption">
                        <parameters>
                        <label>Lieferschein für großes Paket drucken</label>
                        <method>:post</method>
                        <arguments>
                            <reference>!data.palletID</reference>
                            <logisticOption>!data.logisticOption</logisticOption>
                        </arguments>
                        </parameters>
                        <code>
                        <prepare>data.logisticOption = "Palette";</prepare>
                        <finalize output="result"/>
                        <update output="result"/>
                        <rescue output="result"/>
                        </code>
                        <annotations>
                        <_timing>
                            <_timing_weight/>
                            <_timing_avg/>
                            <explanations/>
                        </_timing>
                        <_shifting>
                            <_shifting_type>Duration</_shifting_type>
                        </_shifting>
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
                    <manipulate id="a34" label="Paket auf Palette beladen und abwarten bis diese voll ist"/>
                    </alternative>
                    <otherwise>
                    <call id="a30" endpoint="logisticOption">
                        <parameters>
                        <label>Expresslieferschein drucken</label>
                        <method>:post</method>
                        <arguments>
                            <reference>!data.palletID</reference>
                            <logisticOption>!data.logisticOption</logisticOption>
                        </arguments>
                        </parameters>
                        <code>
                        <prepare>data.logisticOption = "Express";</prepare>
                        <finalize output="result"/>
                        <update output="result"/>
                        <rescue output="result"/>
                        </code>
                        <annotations>
                        <_timing>
                            <_timing_weight/>
                            <_timing_avg/>
                            <explanations/>
                        </_timing>
                        <_shifting>
                            <_shifting_type>Duration</_shifting_type>
                        </_shifting>
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
                    </otherwise>
                </choose>
                <manipulate id="a27" label="Produkt versenden"/>
                <stop id="a31"/>
                </description>
            </description>
            <transformation>
                <description type="copy"/>
                <dataelements type="none"/>
                <endpoints type="none"/>
            </transformation>
            </testset>`
        }
    },
    methods: {
        toggleInstance() {
            this.opened = !this.opened;
        }
    }
}
</script>

<style scoped>
h3,
span,
p {
    cursor: pointer;
    user-select: none;
}

.heading {
    display: flex;
    justify-content: space-between;
    align-items: center;
}

iframe {
    width: 100%;
    height: 100vh;
}
</style>