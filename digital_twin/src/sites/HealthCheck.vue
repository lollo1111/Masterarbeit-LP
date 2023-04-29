<template>
    <the-card>
        <template #header>Healthcheck</template>
        <template #default>
            <p>
                Lorem, ipsum dolor sit amet consectetur adipisicing elit. Repellendus autem provident ipsam et inventore
                alias necessitatibus beatae perferendis, ratione placeat.
            </p>
            <ul>
                <li>
                    <div class="service">
                        <div class="heading">
                            <span>⚙️</span>
                            <span :class="{healthy: factory === 'healthy', unhealthy: factory === 'unhealthy'}">{{ factory.toUpperCase() }}</span>
                        </div>
                        <h4>Factory I/O API</h4>
                        <div class="line"></div>
                        <p>Web API der 3D-Simulation. Indem Factory I/O geöffnet wird, sowie die erforderliche Konfigurationsdatei im Installationsordner hinzugefügt wurde, wird diese automatisch gestartet.</p>
                    </div>
                </li>
                <li>
                    <div class="service">
                        <div class="heading">
                            <span>⚙️</span>
                            <span :class="{healthy: sdk === 'healthy', unhealthy: sdk === 'unhealthy'}">{{ sdk.toUpperCase() }}</span>
                        </div>
                        <h4>Factory I/O SDK</h4>
                        <div class="line"></div>
                        <p>Web Development Kid der 3D-Simulation. Läuft lokal auf dem Gerät und nicht in Docker. Benötigt .NET Runtime sowie SDK.</p>
                    </div>
                </li>
                <li>
                    <div class="service">
                        <div class="heading">
                            <span>⚙️</span>
                            <span :class="{healthy: simulation === 'healthy', unhealthy: simulation === 'unhealthy'}">{{ simulation.toUpperCase() }}</span>
                        </div>
                        <h4>Simulation API</h4>
                        <div class="line"></div>
                        <p>Backend Simulations-API zum Abfragen aktueller Werte sowie zur Ausführung der Simulation anhand verfügbarer Endpoints.</p>
                    </div>
                </li>
                <li>
                    <div class="service">
                        <div class="heading">
                            <span>⚙️</span>
                            <span :class="{healthy: cpee === 'healthy', unhealthy: cpee === 'unhealthy'}">{{ cpee.toUpperCase() }}</span>
                        </div>
                        <h4>WfMS (CPEE)</h4>
                        <div class="line"></div>
                        <p>Workflow Management System zur Steuerung des digitalen Zwillings. Ermöglicht die Nutzung der Workflow Engine sowie eines UIs.</p>
                    </div>
                </li>
                <li>
                    <div class="service">
                        <div class="heading">
                            <span>⚙️</span>
                            <span :class="{healthy: openplc === 'healthy', unhealthy: openplc === 'unhealthy'}">{{ openplc.toUpperCase() }}</span>
                        </div>
                        <h4>PLC (OpenPLC)</h4>
                        <div class="line"></div>
                        <p>Programmable Logic Controller der digitalen Fertigungsanlage. Dient der Automatisierung indem Abläufe anhand von Regeln gesteuert werden.</p>
                    </div>
                </li>
                <li>
                    <div class="service">
                        <div class="heading">
                            <span>⚙️</span>
                            <span :class="{healthy: mqtt === 'healthy', unhealthy: mqtt === 'unhealthy'}">{{ mqtt.toUpperCase() }}</span>
                        </div>
                        <h4>Mosquitto</h4>
                        <div class="line"></div>
                        <p>Leichtgewichtiges IoT-Kommunikationsprotokoll: verwaltet Nachrichten von IoT Geräten.</p>
                    </div>
                </li>
                <li>
                    <div class="service">
                        <div class="heading">
                            <span>⚙️</span>
                            <span :class="{healthy: kafka === 'healthy', unhealthy: kafka === 'unhealthy'}">{{ kafka.toUpperCase() }}</span>
                        </div>
                        <h4>Kafka</h4>
                        <div class="line"></div>
                        <p>Event-Streaming Plattform: speichert und verarbeitet Datenströme.</p>
                    </div>
                </li>
                <li>
                    <div class="service">
                        <div class="heading">
                            <span>⚙️</span>
                            <span :class="{healthy: bridge === 'healthy', unhealthy: bridge === 'unhealthy'}">{{ bridge.toUpperCase() }}</span>
                        </div>
                        <h4>MQTT Bridge</h4>
                        <div class="line"></div>
                        <p>Ermöglicht eine End-to-End Integration zwischen MQTT und Kafka.</p>
                    </div>
                </li>
                <li>
                    <div class="service">
                        <div class="heading">
                            <span>⚙️</span>
                            <span :class="{healthy: influx === 'healthy', unhealthy: influx === 'unhealthy'}">{{ influx.toUpperCase() }}</span>
                        </div>
                        <h4>InfluxDB</h4>
                        <div class="line"></div>
                        <p>In jener Echtzeitdatenbank werden sämtliche Daten der Simulation gespeichert. In weiterer Folge können Analysen durchgeführt werden.</p>
                    </div>
                </li>
                <li>
                    <div class="service">
                        <div class="heading">
                            <span>⚙️</span>
                            <span :class="{healthy: grafana === 'healthy', unhealthy: grafana === 'unhealthy'}">{{ grafana.toUpperCase() }}</span>
                        </div>
                        <h4>Grafana</h4>
                        <div class="line"></div>
                        <p>Ein Service, welcher die angesammelten Daten von IoT Geräten in der Echtzeitdatenbank InfluxDB in Form eines Dashboards aufbereitet.</p>
                    </div>
                </li>
            </ul>
        </template>
    </the-card>
</template>

<script>
export default {
    data() {
        return {
            factory: "unhealthy",
            simulation: "unhealthy",
            influx: "unhealthy",
            grafana: "unhealthy",
            mqtt: "unhealthy",
            kafka: "unhealthy",
            bridge: "unhealthy",
            openplc: "unhealthy",
            cpee: "unhealthy",
            sdk: "unhealthy"
        }
    }
}
</script>

<style scoped>
.heading {
    display: flex;
    justify-content: space-between;
    align-items: center;
}
ul {
    width: 80%;
    margin: 3rem auto;
    list-style: none;
    display: grid;
    grid-template-columns: repeat(5, 1fr);
    grid-auto-rows: 1fr;
    gap: 1.5rem;
}

ul li {
    padding: .5rem;
    border: 1px solid #000;
    border-radius: 5px;
}

ul li:hover {
    background: #ccc;
    cursor: context-menu;
    user-select: none;
}

ul li p {
    hyphens: auto;
}

h4 {
    margin-top: .5rem;
    text-align: center;
}

.healthy {
    padding: .2rem 1rem;
    background: green;
    border: none;
    border-radius: 80px;
    color: #fff;
    font-size: .8rem;
}

.unhealthy {
    font-size: .8rem;
    padding: .2rem 1rem;
    background: red;
    border: none;
    border-radius: 80px;
    color: #fff;
}

.line {
    width: 20%;
    height: 3px;
    background: #7c7a7a;
    margin: .3rem auto .2rem;
}

@media only screen and (max-width: 1200px) {
  ul {
    grid-template-columns: repeat(2, 1fr);
    width: 100%;
  }
}
</style>