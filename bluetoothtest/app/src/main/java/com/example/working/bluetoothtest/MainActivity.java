package com.example.working.bluetoothtest;

import android.bluetooth.BluetoothDevice;
import android.bluetooth.BluetoothSocket;
import android.support.v7.app.AppCompatActivity;
import android.os.Bundle;
import android.view.View;
import android.bluetooth.BluetoothAdapter;
import android.content.Intent;
import java.io.*;
import java.util.UUID;


public class MainActivity extends AppCompatActivity {
    private static final int REQUEST_ENABLE_BT = 1;
    BluetoothAdapter m_BluetoothAdapter;

    private class ConnectThread extends Thread {
        private final BluetoothSocket mmSocket;
        private final BluetoothDevice mmDevice;

        public ConnectThread(BluetoothDevice device) {
            BluetoothSocket tmp = null;
            mmDevice = device;

            try {
                tmp = device.createRfcommSocketToServiceRecord(UUID.fromString("B62C4E8D-62CC-404b-BBBF-BF3E3BBB1374"));
            } catch (IOException e) {

            }
            mmSocket = tmp;
        }

        public void run() {
            m_BluetoothAdapter.cancelDiscovery();

            try {
                mmSocket.connect();
            } catch (IOException connectException) {
                try {
                    mmSocket.close();
                } catch (IOException closeException) { }
                return;
            }

            OutputStream sout;
            try {
                sout = mmSocket.getOutputStream();
                String s = "speed, 1\n"; // speed2, x, 14
                byte[] x = s.getBytes("UTF8");
                sout.write(x);
                    try {
                        Thread.currentThread().sleep(1000);
                    } catch (InterruptedException e) {

                    }

                    String s2 = "speed, 20.3\n";
                    byte[] x2 = s2.getBytes("UTF8");
                    sout.write(x2);
                    try {
                        Thread.currentThread().sleep(1000);
                    } catch (InterruptedException e) {

                    }

                    String s3 = "speed, 10.3\n";
                    byte[] x3 = s3.getBytes("UTF8");
                    sout.write(x3);

                    try {
                        Thread.currentThread().sleep(1000);
                    } catch (InterruptedException e) {

                    }

                    String s4 = "speed, 30.3\n";
                    byte[] x4 = s4.getBytes("UTF8");
                    sout.write(x4);


                    try {
                        Thread.currentThread().sleep(1000);
                    } catch (InterruptedException e) {

                    }

                    String s5 = "speed, 40.3\n";
                    byte[] x5 = s5.getBytes("UTF8");
                    sout.write(x5);


            }
            catch(IOException s) {

            }
        }

        public void cancel() {
            try {
                mmSocket.close();
            } catch (IOException e) {

            }
        }
    }


    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);

        m_BluetoothAdapter = BluetoothAdapter.getDefaultAdapter();
    }

    protected void onClick(View view) {
        if (!m_BluetoothAdapter.isEnabled()) {
            Intent enableBtIntent = new Intent(BluetoothAdapter.ACTION_REQUEST_ENABLE);
            startActivityForResult(enableBtIntent, REQUEST_ENABLE_BT);
        }

        //Todo: Bluetooth MAC Address of the computer running the server goes here
        ConnectThread cThread = new ConnectThread(m_BluetoothAdapter.getRemoteDevice(""));
        cThread.run();
    }
}
