<template>
    <the-card>
        <template #header><span class="underline">Masterarbeit</span>: Die Steuerung von digitalen Zwillingen mit
            Workflow-Management-Systemen am Beispiel einer virtuellen Fertigungslinie</template>
        <template #default>
            <p class="alignright">&copy; Lorenz Pircher (2023)</p>
            <p>
            <p class="description">
                <strong>Kurzfassung:</strong> Digitale Zwillinge gelten als essenziell für die Realisierung der Industrie
                4.0 und werden von vielen Experten als Schlüsseltechnologie dieser angesehen. Sie ermöglichen das Abrufen
                von Echtzeitdaten, die digitale Planung von Diensten oder Produktionen, die Durchführung virtueller
                Experimente und Simulationen sowie Prozessoptimierungen. Aufgrund fehlender Standards, Frameworks,
                Regulierungen und weiterer Herausforderungen befindet sich das Konzept der digitalen Zwillinge jedoch noch
                in der Startphase und bleibt ein größtenteils theoretisches Konzept. Hierbei können Parallelen zu Workflow
                Systemen, mittels Softwaresystem vollständig oder zum Teil automatisierte Prozesse, gezogen werden, da der
                Einsatz von Geschäftsprozessmanagement ebenso Prozessoptimierungen, wie mögliche Kostensenkungen,
                Verkürzungen der Durchführungszeiten sowie Verringerungen der Fehlerquote mit sich bringen kann.<br>
                Diese Abschlussarbeit zielt darauf ab, zu untersuchen, wie digitale Zwillinge und
                Workflow-Management-Systeme (WfMS) gemeinsam genutzt und miteinander vernetzt werden können. Die
                Methodik umfasst eine umfangreiche Literaturrecherche zu den Hintergrundinformationen beider Technologien,
                wobei insbesondere Anwendungsbereiche beleuchtet werden, in denen sich jene Technologien ergänzen. Darüber
                hinaus wird ein Konzept sowie ein Implementierungsansatz vorgestellt, welche die Durchführung zweier
                konkreter Anwendungsfälle einer Fertigungsanlage ermöglichen. Jene Implementierung erlaubt es, einen
                prozessbasierten digitalen Zwilling mithilfe eines WfMS zu steuern. Dieser Zwilling kann unter
                anderem 3D-Simulationen durchführen sowie Echtzeitdaten in Form eines Dashboards aufbereiten. Zudem wird
                eine Web Anwendung entwickelt, welche Benutzern eine intuitive Schnittstelle bietet, um Simulationen zu
                definieren, zu verwalten und auszuführen. Diese Simulationen werden vom WfMS gesteuert, welches
                wiederum den digitalen Zwilling kontrolliert. Das vorgestellte Konzept ermöglicht eine verbesserte
                Zusammenarbeit, auch für Nicht-Experten, da die zugrundeliegenden Prozesse des digitalen Zwillings in einer
                allgemein verständlichen Notation vorliegen und einfach angepasst werden können.
            </p>
            📙 <a target="_blank" href="https://ucloud.univie.ac.at/index.php/s/965HnNpXZ33FxOl"><strong> Masterarbeit als PDF</strong></a><br><br>
            🧑🏻‍💻 <a target="_blank" href="https://github.com/lollo1111/Masterarbeit-Lorenz-Pircher"><strong>Github
                    Repository</strong></a><br><br>
            🎦 <a target="_blank" href="https://ucloud.univie.ac.at/index.php/s/A6u4oLdFpVpIIQ8">
                <strong>Simulations-Aufnahme</strong></a>
            </p>
        </template>
    </the-card>
</template>

<style scoped>
.alignright {
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