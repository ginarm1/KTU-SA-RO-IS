let calendarEl = document.getElementById('calendar');

let url = 'https://localhost:44330/calendar';

let eventsArr = [];

let eventsTable = document.getElementById('eventsTable');
let trElems = eventsTable.getElementsByTagName('tr');

// Get the events from table, put them in javascript array,
// show in calendar and then hide the table
for (let tr of trElems) {
    let tdElems = tr.getElementsByTagName('td');
    const tempStartDate = new Date(tdElems[2].innerText);
    tempStartDate.setDate(tempStartDate.getDate() + 1)
    tdElems[2].innerText = tempStartDate.toISOString().split('T')[0]
    const tempEndDate = new Date(tdElems[3].innerText);
    tempEndDate.setDate(tempEndDate.getDate() + 2)
    tdElems[3].innerText = tempEndDate.toISOString().split('T')[0]

    let event = {
        id: tdElems[0].innerText,
        title: tdElems[1].innerText,
        start: tdElems[2].innerText,
        end: tdElems[3].innerText,
        backgroundColor: tdElems[4].innerText,
        borderColor: tdElems[4].innerText,
    };
    eventsArr.push(event)
}

//FullCalendar parameters
let calendar = new FullCalendar.Calendar(calendarEl, {
    initialView: 'dayGridMonth',
    headerToolbar: {
        left: 'prevYear prev today',
        center: 'title',
        right: 'next nextYear'
        //right: 'dayGridMonth'
    },
    locale: 'lt',
    events: eventsArr,
    eventClick: (info) => {
        toggleEvent(info.event.id)
    },
    weekNumberCalculation: 'ISO'
})

calendar.render();
eventsTable.style.display = "none";

let toggleEvent = (id) => {
    let eventCont = document.getElementById('eventContainer-' + id).style;
    eventCont.display === 'none' ? eventCont.display = 'block' : eventCont.display = 'none';
}