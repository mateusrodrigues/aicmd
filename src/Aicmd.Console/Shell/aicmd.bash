#!/usr/bin/env bash
# aicmd shell integration for bash
#
# Source this in your ~/.bashrc:
#   source /path/to/aicmd.bash
#
# Press Ctrl+G at the bash prompt to invoke aicmd. The generated command will
# be placed in the readline buffer, ready to edit and run with Enter.
#
# To change the key binding, replace '\C-g' below with your preferred binding
# (e.g. '\C-b' for Ctrl+B, '\M-a' for Alt+A).

_aicmd_widget() {
    local old_stty output exit_code

    # Save terminal settings and restore cooked mode so the user can type
    # their description with normal echo and line editing.
    old_stty=$(stty -g 2>/dev/null)
    stty sane 2>/dev/null

    # Run the program. Its prompt goes to stderr (the terminal); its output
    # (the generated command) is captured.
    output=$(aicmd 2>/dev/tty)
    exit_code=$?

    # Restore readline's terminal settings.
    stty "$old_stty" 2>/dev/null

    if [[ $exit_code -eq 0 && -n "$output" ]]; then
        READLINE_LINE="$output"
        READLINE_POINT=${#READLINE_LINE}
    fi
}

bind -x '"\C-g": _aicmd_widget'
