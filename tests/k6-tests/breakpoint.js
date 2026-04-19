import http from 'k6/http';
import { sleep } from 'k6';

export const options = {
    executor: 'ramping-arrival-rate',
    startRate: 10,
    timeUnit: '1s',
    preAllocatedVUs: 100,
    maxVUs: 1000,
    stages: [
        { duration: '15m', target: 500 },
    ],
};

const BASE_URL = 'http://localhost:5063';

export default function () {
    http.get(`${BASE_URL}/api/flights`);

    const payload = JSON.stringify({
        flightId: '00000000-0000-0000-0000-000000000000',
        passengerId: '00000000-0000-0000-0000-000000000000',
        seatNumber: `BP-${Math.floor(Math.random() * 1000000)}`
    });
    http.post(`${BASE_URL}/api/bookings`, payload, { headers: { 'Content-Type': 'application/json' } });
}