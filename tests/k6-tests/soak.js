import http from 'k6/http';
import { check, sleep } from 'k6';

export const options = {
    stages: [
        { duration: '2m', target: 40 }, 
        { duration: '30m', target: 40 },
        { duration: '2m', target: 0 }, 
    ],
};

const BASE_URL = 'http://localhost:5063';

export default function () {
    http.get(`${BASE_URL}/api/flights?origin=Rivne`);
    sleep(2);

    const payload = JSON.stringify({
        flightId: '144e325c-6a09-43f9-879b-b0242fd1f844',
        passengerId: '0e5a7f3c-71df-4cd1-948f-1d1b862f88ff',
        seatNumber: `SK-${Math.floor(Math.random() * 100000)}`
    });
    http.post(`${BASE_URL}/api/bookings`, payload, { headers: { 'Content-Type': 'application/json' } });
    sleep(2);
}