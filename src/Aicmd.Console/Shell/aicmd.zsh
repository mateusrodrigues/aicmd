#!/usr/bin/env zsh
# aicmd shell integration for zsh
#
# Source this in your ~/.zshrc:
#   source /path/to/aicmd.zsh
#
# Press Ctrl+G at the zsh prompt to invoke aicmd. The generated command will
# be placed in the ZLE buffer, ready to edit and run with Enter.
#
# To change the key binding, replace '^G' below with your preferred binding
# (e.g. '^B' for Ctrl+B, '^[a' for Alt+A).

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

    # Restore ZLE's terminal settings.
    stty "$old_stty" 2>/dev/null

    if [[ $exit_code -eq 0 && -n "$output" ]]; then
        BUFFER="$output"
        CURSOR=${#BUFFER}
        zle reset-prompt
    fi
}

zle -N _aicmd_widget
bindkey '^G' _aicmd_widget
