import http from 'k6/http';
import { check, sleep } from 'k6';

export const options = {
    stages: [
        { duration: '10s', target: 300 },
        { duration: '1m', target: 300 }, 
        { duration: '10s', target: 0 }, 
    ],
};

const BASE_URL = 'http://localhost:5063';

export default function () {
    http.get(`${BASE_URL}/api/flights?origin=Kyiv`);

    const payload = JSON.stringify({
        flightId: '144e325c-6a09-43f9-879b-b0242fd1f844',
        passengerId: '0e5a7f3c-71df-4cd1-948f-1d1b862f88ff',
        seatNumber: `SP-${Math.floor(Math.random() * 100000)}`
    });
    http.post(`${BASE_URL}/api/bookings`, payload, { headers: { 'Content-Type': 'application/json' } });
    sleep(1);
}