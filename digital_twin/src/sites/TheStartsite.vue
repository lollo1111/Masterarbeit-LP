<template>
    <the-card>
        <template #header><span class="underline">Masterarbeit</span>: Die Steuerung von digitalen Zwillingen mit
            Workflow-Management-Systemen am Beispiel von Fertigungsprozessen</template>
        <template #default>
            <p class="alignright">&copy; Lorenz Pircher (2023)</p>
            <p>
            <p class="description">
                <strong>Kurzfassung:</strong> Schlüsseltechnologie und technologischer Trend, essenziell für die Realisierung der Industrie 4.0: digitale
                Zwillinge <stron>b1,b10,b11</stron>. Deren Konzept und zugrunde liegende Technologien befinden sich bis dato
                noch in der Startphase ihres Lebenszyklus <strong>b17</strong>.
                Inexistente Standards, Frameworks und Regulierungen sowie weitere Herausforderungen sind dafür
                verantwortlich, warum digitale Zwillinge immer noch ein größtenteils theoretisches Konzept
                <strong>b6,b8</strong>. Die Realisierung solcher ermöglicht unter anderem das Abrufen von Echtzeitdaten, die
                digitale Planung von Diensten oder Produktionen, die Durchführung virtueller Experimente und Simulationen
                sowie Prozessoptimierungen \cite{b1}. Hierbei können Parallelen zu Workflow Systemen, mittels Softwaresystem
                vollständig oder zum Teil automatisierte Prozesse, gezogen werden, da der Einsatz von
                Geschäftsprozessmanagement ebenso Prozessoptimierungen, wie mögliche Kostensenkungen, Verkürzungen der
                Durchführungszeiten sowie Verringerungen der Fehlerquote mit sich bringen kann \cite{b22}.\\
                Diese wissenschaftliche Arbeit zielt darauf ab, zu untersuchen, wie digitale Zwillinge und
                Workflow-Management-Systeme (WfMS) gemeinsam genutzt und miteinander vernetzt werden können. Die Methodik
                umfasst eine umfangreiche Literaturrecherche zu den Hintergrundinformationen beider Technologien, wobei
                insbesondere Anwendungsbereiche beleuchtet werden, in denen sich jene Technologien ergänzen. Darüber hinaus
                wird ein Konzept sowie ein Implementierungsansatz vorgestellt, welche die Durchführung zweier konkreter
                Anwendungsfälle einer Fertigungsanlage ermöglichen. Jene Implementierung erlaubt es, einen prozessbasierten
                digitalen Zwilling mithilfe eines \acrshort{wfms} zu steuern. Dieser Zwilling kann unter anderem
                3D-Simulationen durchführen sowie Echtzeitdaten in Form eines Dashboards aufbereiten. Zudem wird eine
                Webanwendung entwickelt, welche Benutzern eine intuitive Schnittstelle bietet, um Simulationen zu
                definieren, zu verwalten und auszuführen. Diese Simulationen werden vom WfMS gesteuert, welches wiederum den
                digitalen Zwilling kontrolliert. Das vorgestellte Konzept ermöglicht eine verbesserte Zusammenarbeit, auch
                für Nicht-Experten, da die zugrundeliegenden Prozesse des digitalen Zwillings in einer allgemein
                verständlichen Notation vorliegen und einfach angepasst werden können.
            </p>
            📙 <a target="_blank" href="#"><strong> Masterarbeit als PDF</strong></a><br><br>
            🧑🏻‍💻 <a target="_blank" href="https://github.com/lollo1111/Masterarbeit-Lorenz-Pircher"><strong>Github
                    Repository</strong></a><br><br>
            🎦 <a target="_blank" href="https://ucloud.univie.ac.at/index.php/s/9Gn4INE0hNHBDu8">
                <strong>Simulation-Aufnahme</strong></a>
            </p>
        </template>
    </the-card>
</template>

<style scoped>.alignright {
    text-align: end;
}

.description {
    margin: 2rem 0;
}

a {
    color: #000;
    text-decoration: underline 1px solid transparent;
}

a:hover {
    text-decoration-color: #000;
}

.description {
    font-size: 1.2rem;
}

.underline {
    text-decoration: underline dotted 3px #000;
    font-weight: 800;
    text-underline-offset: 5px;
}</style>