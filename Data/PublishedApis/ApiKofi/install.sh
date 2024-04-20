#!/bin/bash
        echo 'Installing ApiKofi...'
        sudo cp ApiKofi.service /etc/systemd/system/
        if sudo systemctl enable ApiKofi && sudo systemctl start ApiKofi; then
            echo 'Installation completed successfully.'
        else
            echo 'Failed to install and start ApiKofi.'
        fi
        read -p 'Press any key to continue...'