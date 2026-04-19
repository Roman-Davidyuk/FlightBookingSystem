import http from 'k6/http';
import { check, sleep } from 'k6';

export const options = {
    vus: 1,
    duration: '30s',
    thresholds: {
        http_req_failed: ['rate==0.00'],
        http_req_duration: ['p(95)<300'],
    },
};

const BASE_URL = 'http://localhost:5063';

export function setup() {

    let flightRes = http.get(`${BASE_URL}/api/flights`);
    let flights = flightRes.json();
    let validFlightId = flights.length > 0 ? flights[0].id : null;

    let passengerRes = http.get(`${BASE_URL}/api/passengers`);
    let passengers = passengerRes.json();
    let validPassengerId = passengers.length > 0 ? passengers[0].id : null;

    return { flightId: validFlightId, passengerId: validPassengerId };
}

export default function (data) {
    if (!data.flightId || !data.passengerId) {
        console.error("❌ Не вдалося отримати ID з API. Перевір, чи є дані в базі!");
        return;
    }

    let searchRes = http.get(`${BASE_URL}/api/flights?origin=Kyiv`);
    check(searchRes, { 'search is 200': (r) => r.status === 200 });
    sleep(1);

    const payload = JSON.stringify({
        flightId: data.flightId,
        passengerId: data.passengerId,
        seatNumber: `SM-${Math.floor(Math.random() * 10000)}`
    });

    let bookRes = http.post(`${BASE_URL}/api/bookings`, payload, {
        headers: { 'Content-Type': 'application/json' }
    });

    check(bookRes, { 'booking is 201 or 400': (r) => [201, 400].includes(r.status) });
    sleep(1);
}