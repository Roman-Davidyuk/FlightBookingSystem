import http from 'k6/http';
import { check, sleep } from 'k6';

export const options = {
    stages: [
        { duration: '1m', target: 50 },
        { duration: '3m', target: 50 },
        { duration: '1m', target: 0 },
    ],
    thresholds: {
        http_req_failed: ['rate<0.01'],
        http_req_duration: ['p(95)<500'],
    },
};

const BASE_URL = 'http://localhost:5063';

export default function () {
    let res = http.get(`${BASE_URL}/api/flights?origin=Lviv`);
    check(res, { 'search OK': (r) => r.status === 200 });
    sleep(1);

    const payload = JSON.stringify({
        flightId: '00000000-0000-0000-0000-000000000000',
        passengerId: '00000000-0000-0000-0000-000000000000',
        seatNumber: `LD-${Math.floor(Math.random() * 10000)}`
    });
    res = http.post(`${BASE_URL}/api/bookings`, payload, { headers: { 'Content-Type': 'application/json' } });
    check(res, { 'booking handled': (r) => r.status !== 500 });
    sleep(Math.random() * 2);
}